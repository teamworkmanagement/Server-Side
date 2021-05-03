using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;

namespace TeamApp.WebApi.Hubs.Post
{
    public interface IHubPostClient
    {
        Task NewComment(CommentRequest message);
    }
}
