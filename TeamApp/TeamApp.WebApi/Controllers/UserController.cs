using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;


namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var outPut = await _repo.GetAll();
            return Ok(outPut);
        }
    }
}
