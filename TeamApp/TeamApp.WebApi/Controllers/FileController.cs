using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.File;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileRepository _repo;
        public FileController(IFileRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetById(string fileId)
        {
            return Ok(await _repo.GetById(fileId));
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> AddFile(string taskId, [FromForm] FileRequest fileReq)
        {
            return Ok(await _repo.AddFile(taskId, fileReq));
        }
    }
}
