using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/comment-report")]
    public class CommentReportController: ControllerBase
    {
        private readonly ICommentReportRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public CommentReportController(ICommentReportRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        
        [HttpPost]
        public async Task<IActionResult> AddReport([FromBody] CreateCommentReport createReportRequest)
        {
            createReportRequest.UserId = _authenticatedUserService.UserId;
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
            return Ok(new ApiResponse<List<CommentReportResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromReport([FromBody] CommentListWrap reportListWrap)
        {
            var outPut = await _repo.RemoveFromReport(reportListWrap.CommentIds);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("accept")]
        public async Task<IActionResult> ChangeCommentStatusAccept([FromBody] CommentListWrap commentListWrap)
        {
            var outPut = await _repo.ChangeCommentStatusAccept(commentListWrap.CommentIds);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("deny")]
        public async Task<IActionResult> ChangeCommentStatusDeny([FromBody] CommentListWrap commentListWrap)
        {
            var outPut = await _repo.ChangeCommentStatusDeny(commentListWrap.CommentIds);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
    }
}
