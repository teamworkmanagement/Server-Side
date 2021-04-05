using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Tag;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/tag")]
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
            var res = await _repo.GetById(tagId);

            var outPut = new ApiResponse<TagObject>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Tag không tồn tại" : null,
            };

            return Ok(outPut);
        }

        [HttpPost]
        public async Task<IActionResult> AddTag([FromForm] TagObject tagObj)
        {
            var res = await _repo.AddTag(tagObj);

            var outPut = new ApiResponse<string>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Thêm thất bại" : null,
            };

            return Ok(outPut);
        }
    }
}
