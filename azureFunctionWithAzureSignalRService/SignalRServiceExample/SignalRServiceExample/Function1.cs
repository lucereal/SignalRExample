using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
using Azure;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace SignalRServiceExample
{
    public class Function1
    {
        private readonly ILogger _logger;
        private static readonly HttpClient HttpClient = new();
        private static string Etag = string.Empty;
        private static int StarCount = 0;
        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }


        [Function("indexStar")]
        public static HttpResponseData GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous, Route = "star/index")] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(File.ReadAllText("content/index.html"));
            response.Headers.Add("Content-Type", "text/html");
            return response;
        }

        [Function("negotiateStar")]
        public static HttpResponseData Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, Route = "star/negotiate")] HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = "serverless")] string connectionInfo)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(connectionInfo);
            return response;
        }

        [Function("broadcastStar")]
        [SignalROutput(HubName = "serverless")]
        public static async Task<SignalRMessageAction> Broadcast([TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/azure/azure-signalr");
            request.Headers.UserAgent.ParseAdd("Serverless");
            request.Headers.Add("If-None-Match", Etag);
            var response = await HttpClient.SendAsync(request);
            if (response.Headers.Contains("Etag"))
            {
                Etag = response.Headers.GetValues("Etag").First();
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<GitResult>();

                
                if (result != null)
                {
                    StarCount = result.StarCount;
                }
            }
            return new SignalRMessageAction("newMessage", new object[] { $"Current star count of https://github.com/Azure/azure-signalr is: {StarCount}" });
        }

        private class GitResult
        {
            [JsonPropertyName("stargazers_count")]
            public int StarCount { get; set; }
        }

       
    }
}
