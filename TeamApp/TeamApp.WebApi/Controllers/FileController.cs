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
    [Authorize]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IFileRepository _repo;
        public FileController(IFileRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get file by id API
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<FileResponse>), 200)]
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

        /*[HttpPost("{taskId}")]
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
        }*/

        /// <summary>
        /// Get file pagination in upload file API
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<FileResponse>), 200)]
        public async Task<IActionResult> GetFileByBelonged([FromQuery] FileRequestParameter parameter)
        {
            var outPut = await _repo.GetByBelong(parameter);
            return Ok(new ApiResponse<PagedResponse<FileResponse>>
            {
                Succeeded = true,
                Data = outPut,
            }); ;
        }

        /// <summary>
        /// Add file API
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FileResponse>), 200)]
        public async Task<IActionResult> AddFile(FileRequest fileRequest)
        {
            var outPut = await _repo.AddFile(fileRequest);
            return Ok(new ApiResponse<FileResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Add image of post API
        /// </summary>
        /// <param name="postFileUploadRequest"></param>
        /// <returns></returns>
        [HttpPost("upload-images-post")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> UploadImagesForPost(PostFileUploadRequest postFileUploadRequest)
        {
            var outPut = await _repo.UploadImageForPost(postFileUploadRequest);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        /// <summary>
        /// Get pagination file specify owner id API
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(ApiResponse<List<FileResponse>>), 200)]
        public async Task<IActionResult> GetAllFiles([FromQuery] FileRequestParameter parameter)
        {
            var outPut = await _repo.GetAll(parameter);
            return Ok(new ApiResponse<List<FileResponse>>
            {
                Succeeded = true,
                Data = outPut,
            }); ;
        }

        /// <summary>
        /// Change file from team to personal API
        /// </summary>
        /// <param name="copyFileToUserModel"></param>
        /// <returns></returns>
        [HttpPost("copy-file")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> CopyFile(CopyFileToUserModel copyFileToUserModel)
        {
            var outPut = await _repo.CopyFileToUser(copyFileToUserModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
    }
}
