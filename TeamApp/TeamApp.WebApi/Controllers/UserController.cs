using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Entities;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly KhoaLuanContext _dbContext;
        public UserController(KhoaLuanContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("/getall")]
        public async Task<IActionResult> GetAll()
        {
            var outPut = await _dbContext.User.ToListAsync();
            return Ok(outPut);
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {            
            return Ok();
        }
    }
}
