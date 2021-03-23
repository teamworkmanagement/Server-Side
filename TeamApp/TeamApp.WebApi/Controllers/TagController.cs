using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Tag;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _repo;
        public TagController(ITagRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{tagId}")]
        public async Task<IActionResult> GetById(string tagId)
        {
            return Ok(await _repo.GetById(tagId));
        }

        [HttpPost]
        public async Task<IActionResult> AddTag([FromForm] TagObject tagObj)
        {
            return Ok(await _repo.AddTag(tagObj));
        }
    }
}
