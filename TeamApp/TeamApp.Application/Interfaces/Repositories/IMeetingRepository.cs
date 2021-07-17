using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Meeting;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IMeetingRepository
    {
        Task<MeetingResponse> AddMeeting(MeetingRequest meetingRequest);
        Task<bool> DeleteMeeting(string meetingId);
        Task<List<MeetingResponse>> GetMeetingByTeam(string teamId);
        Task<bool> JoinMeeting(JoinMeetingModel joinMeetingModel);
        Task<bool> LeaveMeeting(LeaveMeetingModel leaveMeetingModel);
        Task<bool> InviteMembers(InviteMemberModel inviteMemberModel);
        Task<MeetingResponse> GetById(string meetingId);
    }
}
