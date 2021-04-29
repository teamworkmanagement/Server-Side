using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.File;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/file")]
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
            var res = await _repo.GetById(fileId);

            var outPut = new ApiResponse<FileResponse>
            {
                Data = res == null ? null : res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "File không tồn tại" : null,
            };

            return Ok(outPut);
        }

        [HttpPost("{taskId}")]
        public async Task<IActionResult> AddFileTask(string taskId, [FromForm] FileRequest fileReq)
        {
            var res = await _repo.AddFileTask(taskId, fileReq);

            var outPut = new ApiResponse<string>
            {
                Data = res != null ? res : null,
                Message = res != null ? null : "Thêm không thành công",
                Succeeded = res != null ? true : false,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Get file pagination in upload file
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(FileResponse),200)]
        public async Task<IActionResult> GetTask([FromQuery] FileRequestParameter parameter)
        {
            var outPut = await _repo.GetByBelong(parameter);
            return Ok(new ApiResponse<PagedResponse<FileResponse>>
            {
                Succeeded = true,
                Data = outPut,
            }); ;
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(FileRequest fileRequest)
        {
            var outPut = await _repo.AddFile(fileRequest);
            return Ok(new ApiResponse<string>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }
    }
}
