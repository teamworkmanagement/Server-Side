using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.User;

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

        [HttpGet("getalltest")]
        public IActionResult TestFunc()
        {
            var ctrlName = ControllerContext.ActionDescriptor.ControllerName;
            return Ok(
                new
                {
                    Name = "Nguyen Tien Dung",
                    ControllerName = ctrlName,
                });
        }

        [HttpGet("getbyteamid/{teamId}")]
        public async Task<IActionResult> GetByTeamId(string teamId)
        {
            return Ok(await _repo.GetAllByTeamId(teamId));
        }

        [HttpGet("getbyuserid/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            return Ok(await _repo.GetById(userId));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromForm] UserRequest userReq)
        {
            return Ok(await _repo.AddUser(userReq));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] string userId, [FromForm] UserRequest userReq)
        {
            return Ok(await _repo.UpdateUser(userId, userReq));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            return Ok(await _repo.DeleteUser(userId));
        }
    }
}
