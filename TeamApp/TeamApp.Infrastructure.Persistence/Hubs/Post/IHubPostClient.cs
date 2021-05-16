using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;

namespace TeamApp.Infrastructure.Persistence.Hubs.Post
{
    public interface IHubPostClient
    {
        Task NewComment(CommentRequest message);
        Task NewAddReact(object react);
        Task RemoveReact(object react);
    }
}
