using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/post-report")]
    public class PostReportController : ControllerBase
    {
        private readonly IPostReportRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public PostReportController(IPostReportRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReport([FromBody] CreateReportRequest createReportRequest)
        {
            createReportRequest.UserReportId = _authenticatedUserService.UserId;
            var outPut = await _repo.AddReport(createReportRequest);
            return Ok(new ApiResponse<string>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var outPut = await _repo.GetReports();
            return Ok(new ApiResponse<List<PostReportResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromReport([FromBody] ReportListWrap reportListWrap)
        {
            var outPut = await _repo.RemoveFromReport(reportListWrap.ReportIds);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("accept")]
        public async Task<IActionResult> ChangePostStatusAccept([FromBody] PostListWrap postListWrap)
        {
            var outPut = await _repo.ChangePostStatusAccept(postListWrap.PostIds);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("deny")]
        public async Task<IActionResult> ChangePostStatusDeny([FromBody] PostListWrap postListWrap)
        {
            var outPut = await _repo.ChangePostStatusDeny(postListWrap.PostIds);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
    }
}
