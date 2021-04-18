using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conesoft.Hosting.Tasks
{
    [Route("[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        [HttpPost("{*taskId}")]
        public async Task<IActionResult> Post([FromServices] IConfiguration configuration, [FromServices] IEnumerable<ITask> tasks, [FromBody] JsonContent content, [FromRoute] string taskId)
        {
            var accessToken = configuration["tasks:access-token"];
            if (accessToken == content.AccessToken && tasks.FirstOrDefault(t => t.GetType().Name.ToLowerInvariant() == taskId.Replace(" ", "").ToLowerInvariant()) is ITask task)
            {
                await task.Run();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("all.json")]
        public IActionResult GetAll() => File("wwwroot/tasks/all.json", "text/json");

        public class JsonContent
        {
            public string AccessToken { get; set; }
        }
    }
}