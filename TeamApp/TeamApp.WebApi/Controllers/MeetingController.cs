using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Meeting;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/meeting")]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public MeetingController(IMeetingRepository meetingRepository, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = meetingRepository;
            _authenticatedUserService = authenticatedUserService;
        }
        [HttpPost]
        public async Task<IActionResult> AddMeeting([FromBody] MeetingRequest meetingRequest)
        {
            meetingRequest.UserCreateId = _authenticatedUserService.UserId;
            var outPut = await _repo.AddMeeting(meetingRequest);
            return Ok(new ApiResponse<MeetingResponse>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
        [HttpDelete("{meetingId}")]
        public async Task<IActionResult> DeleteMeeting(string meetingId)
        {
            var outPut = await _repo.DeleteMeeting(meetingId);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetMeetings(string teamId)
        {
            var outPut = await _repo.GetMeetingByTeam(teamId);
            return Ok(new ApiResponse<List<MeetingResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
        [HttpPost("join-meeting")]
        public async Task<IActionResult> JoinMeeting([FromBody] JoinMeetingModel joinMeetingModel)
        {
            joinMeetingModel.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.JoinMeeting(joinMeetingModel);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = outPut,
                Data = outPut,
            });
        }
        [HttpPost("leave-meeting")]
        public async Task<IActionResult> LeaveMeeting([FromBody] LeaveMeetingModel leaveMeetingModel)
        {
            var outPut = await _repo.LeaveMeeting(leaveMeetingModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [HttpPost("invite-meeting")]
        public async Task<IActionResult> InviteMeeting([FromBody] InviteMemberModel inviteMemberModel)
        {
            inviteMemberModel.UserInvite = _authenticatedUserService.UserId;
            var outPut = await _repo.InviteMembers(inviteMemberModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetMeetingRequest getMeetingRequest)
        {
            getMeetingRequest.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.Get(getMeetingRequest);
            return Ok(new ApiResponse<MeetingResponse>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [HttpGet("check-call")]
        public async Task<IActionResult> CheckIsCalling()
        {
            var outPut = await _repo.CheckIsCalling(_authenticatedUserService.UserId);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = true,
                Data = outPut,
            });
        }
    }
}
