using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.DTOs.Notification;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;


namespace TeamApp.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<PagedResponse<NotificationResponse>> GetPaging(NotificationRequestParameter parameter);
        Task<bool> ReadNotificationSet(ReadNotiModel readNotiModel);
        Task PushNotiCommentTag(CommentMentionRequest mentionRequest);
        Task PushNotiAddPostTag(AddPostMentionRequest mentionRequest);
        Task PushNotiJoinTeam(JoinTeamNotification joinTeamNotification);
        Task PushNotiAssignTask(AssignNotiModel assignNotiModel);
    }
}
