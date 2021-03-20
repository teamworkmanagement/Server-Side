using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeamApp.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamApp.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]Product filter)
        {

            return Ok("get all");
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok("get id");
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Update([FromForm] Product product)
        {
            return Ok();
        }
    }
}
