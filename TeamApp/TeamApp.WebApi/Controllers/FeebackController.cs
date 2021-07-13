using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Feedback;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/feedback")]
    public class FeebackController : ControllerBase
    {
        private readonly IFeedbackRepository _repo;
        public FeebackController(IFeedbackRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Add feedback API
        /// </summary>
        /// <param name="feedbackRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackRequest feedbackRequest)
        {
            var outPut = await _repo.AddFeedback(feedbackRequest);

            return Ok(new ApiResponse<string>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetFeedbacks()
        {
            var outPut = await _repo.GetFeedbacks();
            return Ok(new ApiResponse<List<FeedbackResponse>>
            {
                Succeeded = true,
                Data = outPut,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFeedbacks([FromBody] FeedbackUpdateRequest feedbackUpdateRequest)
        {
            var outPut = await _repo.RemoveFeedbacks(feedbackUpdateRequest.FeedbackIds);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = outPut,
                Data = outPut,
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("seen")]
        public async Task<IActionResult> UpdateSeenFeedbacks([FromBody] FeedbackUpdateRequest feedbackUpdateRequest)
        {
            var outPut = await _repo.MakeAsSeen(feedbackUpdateRequest.FeedbackIds);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = outPut,
                Data = outPut,
            });
        }
    }
}
