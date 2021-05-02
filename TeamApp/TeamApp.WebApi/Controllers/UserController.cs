using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
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
            return Ok(new
            {
                Name = "Nguyen Tien Dung",
            });
        }


        [HttpGet("getbyuserid/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            return Ok(await _repo.GetById(userId));
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
