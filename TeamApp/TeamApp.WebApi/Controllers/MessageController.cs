using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _repo;
        public MessageController(IMessageRepository repo)
        {
            _repo = repo;
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
    }
}
