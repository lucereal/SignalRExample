using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Text);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageUser([FromBody] Message message)
        {
            await _hubContext.Clients.All.SendAsync("SendChatMessage", message.User, message.Text);
            
            return Ok();
        }
    }
}
