using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SignalRServiceExample
{
    public class FunctionChat
    {
        private readonly ILogger _logger;
        private static readonly HttpClient HttpClient = new();
        private static string Etag = string.Empty;
 
        public FunctionChat(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }



        [Function("index")]
        public HttpResponseData GetWebPage([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(File.ReadAllText("content/indexchat.html"));
            response.Headers.Add("Content-Type", "text/html");

            return response;
        }

        [Function("foo")]
        [SignalROutput(HubName = "Hub")]
        public SignalRMessageAction foo([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(File.ReadAllText("content/indexchat.html"));
            response.Headers.Add("Content-Type", "text/html");
            return new SignalRMessageAction("newMessage", new object[] { "hello" });

        }

        [Function("Negotiate")]

        public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req,
            [SignalRConnectionInfoInput(HubName = "Hub", UserId = "{query.userid}")] SignalRConnectionInfo signalRConnectionInfo)
        {

            
            _logger.LogInformation("Executing negotiation.");
            return signalRConnectionInfo;
        }


        [Function("OnConnected")]
        [SignalROutput(HubName = "Hub")]
        public SignalRMessageAction OnConnected([SignalRTrigger("Hub", "connections", "connected")] SignalRInvocationContext invocationContext)
        {
            invocationContext.Headers.TryGetValue("Authorization", out var auth);
            _logger.LogInformation($"{invocationContext.ConnectionId} has connected");
            return new SignalRMessageAction("newConnection")
            {
                Arguments = new object[] { new NewConnection(invocationContext.ConnectionId, auth) },

            };
        }


        [Function(nameof(Broadcast))]
        [SignalROutput(HubName = "Hub")]
        public static SignalRMessageAction Broadcast([SignalRTrigger("Hub", "messages", "fish", "message", ConnectionStringSetting = "AzureSignalRConnectionString")] 
        SignalRInvocationContext invocationContext, string message, FunctionContext functionContext)
        {
            return new SignalRMessageAction("newMessage")
            {
                Arguments = new object[] { "FKdjflkajd" }
            };
        }


    //    [Function(nameof(OnClientMessage))]
    //    public static void OnClientMessage(
    //[SignalRTrigger("Hub", "messages", "sendMessage", "content", ConnectionStringSetting = "SignalRConnection")]
    //    SignalRInvocationContext invocationContext, string content, FunctionContext functionContext)
    //    {
    //        var logger = functionContext.GetLogger(nameof(OnClientMessage));
    //        logger.LogInformation("Connection {connectionId} sent a message. Message content: {content}", invocationContext.ConnectionId, content);
    //    }



        public class NewConnection
        {
            public string ConnectionId { get; }

            public string Authentication { get; }

            public NewConnection(string connectionId, string auth)
            {
                ConnectionId = connectionId;
                Authentication = auth;
            }
        }

        public class NewMessage
        {
            public string ConnectionId { get; }
            public string Sender { get; }
            public string Text { get; }

            public NewMessage(SignalRInvocationContext invocationContext, string message)
            {
                Sender = string.IsNullOrEmpty(invocationContext.UserId) ? string.Empty : invocationContext.UserId;
                ConnectionId = invocationContext.ConnectionId;
                Text = message;
            }
        }
    }
}
