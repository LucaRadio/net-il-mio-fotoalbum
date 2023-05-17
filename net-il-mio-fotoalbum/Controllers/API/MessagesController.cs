using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Models;
using NuGet.Protocol;
using System.Text.Json;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        [HttpPost]
        public IActionResult Create([FromBody] Message i)
        {
            try
            {
                using (Context db = new())
                {
                    Message n = new();
                    n.emailSender = i.emailSender;
                    n.messageContent = i.messageContent;
                    n.created_at = DateTime.UtcNow;
                    db.messages.Add(n);
                    db.SaveChanges();
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }


        }

    }
}
