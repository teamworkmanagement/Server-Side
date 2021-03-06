using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.Utils;
using TeamApp.Application.DTOs.User;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Post;
using System.Collections.ObjectModel;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<HubPostClient, IHubPostClient> _postHub;
        private readonly IFileRepository _fileRepo;

        public PostRepository(TeamAppContext dbContext, INotificationRepository notificationRepository, IHubContext<HubPostClient, IHubPostClient> postHub, IFileRepository fileRepo)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
            _postHub = postHub;

            _fileRepo = fileRepo;
        }
        public async Task<PostResponse> AddPost(PostRequest postReq)
        {
            var entity = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                PostUserId = postReq.PostUserId,
                PostTeamId = postReq.PostTeamId,
                PostContent = postReq.PostContent,
                PostCreatedAt = DateTime.UtcNow,
                PostCommentCount = 0,
                PostIsDeleted = false,
            };

            await _dbContext.Post.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync();

            if (postReq.UserIds.Count != 0)
                await _notificationRepository.PushNotiAddPostTag(new AddPostMentionRequest
                {
                    ActionUserId = postReq.PostUserId,
                    UserIds = postReq.UserIds,
                    PostId = entity.PostId,
                });


            if (postReq.PostImages != null && postReq.PostImages.Count != 0)
            {
                var files = postReq.PostImages.Select(x => new File
                {
                    FileId = Guid.NewGuid().ToString(),
                    FileUrl = x.Link,
                    FilePostOwnerId = entity.PostId,
                    FileUploadTime = DateTime.UtcNow,
                });

                await _dbContext.BulkInsertAsync(files);
            }

            if (check > 0)
            {
                var team = await _dbContext.Team.FindAsync(entity.PostTeamId);
                return new PostResponse
                {
                    PostId = entity.PostId,
                    PostUserId = entity.PostUserId,
                    PostTeamId = entity.PostTeamId,
                    PostContent = entity.PostContent,
                    PostCreatedAt = entity.PostCreatedAt,
                    TeamName = team.TeamName,
                };
            }

            return null;
        }

        public async Task<string> AddReact(ReactModel react)
        {
            var entity = new PostReact
            {
                PostReactId = Guid.NewGuid().ToString(),
                PostReactPostId = react.PostId,
                PostReactUserId = react.UserId,
            };


            var teamId = await (from p in _dbContext.Post.AsNoTracking()
                                where p.PostId == react.PostId
                                select p.PostTeamId).FirstOrDefaultAsync();

            var query = from p in _dbContext.Participation.AsNoTracking()
                        join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                        where p.ParticipationTeamId == teamId && uc.Type == "post" && p.ParticipationIsDeleted == false
                        select uc.ConnectionId;

            query = query.Distinct();

            var clients = await query.ToListAsync();

            var readOnlyStr = new ReadOnlyCollection<string>(clients);
            await _postHub.Clients.Clients(readOnlyStr).NewAddReact(new
            {
                react.PostId,
                UserId = react.UserId,
            });

            await _dbContext.PostReact.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.PostReactId;
        }

        public async Task<bool> DeletePost(string postId)
        {
            var entity = await _dbContext.Post.FindAsync(postId);
            if (entity == null)
                return false;

            entity.PostIsDeleted = true;
            _dbContext.Post.Update(entity);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReact(ReactModel react)
        {
            var entity = await _dbContext.PostReact.Where(x => x.PostReactPostId == react.PostId
              && x.PostReactUserId == react.UserId).FirstOrDefaultAsync();
            if (entity == null)
                return false;

            if (entity != null)
                _dbContext.PostReact.Remove(entity);

            var teamId = await (from p in _dbContext.Post.AsNoTracking()
                                where p.PostId == react.PostId
                                select p.PostTeamId).FirstOrDefaultAsync();

            var query = from p in _dbContext.Participation.AsNoTracking()
                        join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                        where p.ParticipationTeamId == teamId && uc.Type == "post" && p.ParticipationIsDeleted == false
                        select uc.ConnectionId;

            query = query.Distinct();

            var clients = await query.ToListAsync();

            var readOnlyStr = new ReadOnlyCollection<string>(clients);
            await _postHub.Clients.Clients(readOnlyStr).RemoveReact(new
            {
                react.PostId,
                UserId = react.UserId,
            });

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResponse<PostResponse>> GetPaging(RequestParameter parameter)
        {
            var queryUserPost = from p in _dbContext.Post
                                join u in _dbContext.User on p.PostUserId equals u.Id
                                select new { p, u.ImageUrl, u.Id, u.FullName, p.Comments.Count };

            var queryOrder = queryUserPost.OrderByDescending(x => x.p.PostCreatedAt);

            var query = queryOrder.Skip((parameter.PageNumber - 1) * parameter.PageSize)
                .Take(parameter.PageSize);

            var entityList = await query.Select(x => new PostResponse
            {
                PostId = x.p.PostId,
                PostUserId = x.p.PostUserId,
                PostTeamId = x.p.PostTeamId,
                PostContent = x.p.PostContent,
                PostCreatedAt = x.p.PostCreatedAt.FormatTime(),
                PostCommentCount = x.Count,
                PostIsDeleted = x.p.PostIsDeleted,
                UserName = x.FullName,
                UserAvatar = x.ImageUrl,
            }).ToListAsync();

            var outPut = new PagedResponse<PostResponse>(entityList, parameter.PageNumber, parameter.PageSize, await query.CountAsync());

            return outPut;
        }

        public async Task<PagedResponse<PostResponse>> GetPostPagingTeam(PostRequestParameter parameter)
        {
            var team = await _dbContext.Team.FindAsync(parameter.TeamId);
            if (team == null)
                throw new KeyNotFoundException("Team not found");
            bool advanced = false;
            if (parameter.FromDate != null || parameter.ToDate != null || !string.IsNullOrEmpty(parameter.GroupId) || !string.IsNullOrEmpty(parameter.PostUser))
                advanced = true;


            //bảng chứa toàn bộ thông tin các post của team
            var query = from p in _dbContext.Post.AsNoTracking()
                        join t in _dbContext.Team.AsNoTracking() on p.PostTeamId equals t.TeamId
                        join u in _dbContext.User on p.PostUserId equals u.Id
                        where t.TeamId == parameter.TeamId && p.PostIsDeleted == false
                        orderby p.PostCreatedAt descending
                        select new { p, u, p.Comments.Count, RCount = p.PostReacts.Count, t.TeamName };

            //tìm kiếm nâng cao
            if (advanced)
            {
                if (parameter.FromDate != null)
                {
                    DateTime dtFrom = (DateTimeOffset.FromUnixTimeMilliseconds((long)parameter.FromDate)).UtcDateTime;
                    query = query.Where(x => x.p.PostCreatedAt >= dtFrom);
                }
                if (parameter.ToDate != null)
                {
                    DateTime dateTo = (DateTimeOffset.FromUnixTimeMilliseconds((long)parameter.ToDate)).UtcDateTime;
                    query = query.Where(x => x.p.PostCreatedAt <= dateTo);
                }
                if (parameter.GroupId != null)
                {
                    query = query.Where(x => x.p.PostTeamId == parameter.GroupId);
                }
                if (parameter.PostUser != null)
                {
                    query = query.Where(x => x.u.Id == parameter.PostUser);
                }
            }

            if (!string.IsNullOrEmpty(parameter.BasicFilter) && !advanced)
            {
                DateTime now = DateTime.UtcNow;
                DateTime baseDate = now.Date;

                switch (parameter.BasicFilter)
                {
                    case BasicFilter.Lastest:
                        query = query.OrderByDescending(x => x.p.PostCreatedAt);
                        break;
                    case BasicFilter.LastHour:
                        var lastHour = now.AddMinutes(-60);
                        query = query.Where(x => x.p.PostCreatedAt >= lastHour && x.p.PostCreatedAt <= now);
                        break;
                    case BasicFilter.Today:
                        query = query.Where(x => ((DateTime)x.p.PostCreatedAt).Date == baseDate);
                        break;
                    case BasicFilter.ThisWeek:
                        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        query = from q in query
                                where q.p.PostCreatedAt >= thisWeekStart && q.p.PostCreatedAt <= thisWeekEnd
                                select q;
                        //query.Where(x => x.p.PostCreatedAt >= flagstart && x.p.PostCreatedAt <= flagend);
                        break;
                    case BasicFilter.ThisMonth:
                        var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        query = from q in query
                                where q.p.PostCreatedAt >= thisMonthStart && q.p.PostCreatedAt <= thisMonthEnd
                                select q;
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(parameter.BasicFilter) && !advanced)
                query = query.OrderByDescending(x => x.p.PostCreatedAt);

            var reactQuery = from q in query
                             from r in _dbContext.PostReact.Where(r => r.PostReactPostId == q.p.PostId && r.PostReactUserId == parameter.UserId).DefaultIfEmpty()
                             select new { q, isReacted = r.PostReactUserId };

            var entityList = await reactQuery.Select(x => new PostResponse
            {
                PostId = x.q.p.PostId,
                PostUserId = x.q.p.PostUserId,
                PostTeamId = x.q.p.PostTeamId,
                PostContent = x.q.p.PostContent,
                PostCreatedAt = x.q.p.PostCreatedAt.FormatTime(),
                PostCommentCount = x.q.Count,
                PostIsDeleted = x.q.p.PostIsDeleted,
                UserName = x.q.u.FullName,
                UserAvatar = x.q.u.ImageUrl,
                PostReactCount = x.q.RCount,
                TeamName = x.q.TeamName,
                IsReacted = x.isReacted == null ? false : true,
            }).Skip(parameter.SkipItems).Take(parameter.PageSize).ToListAsync();


            foreach (var ent in entityList)
            {
                var listImage = await (from f in _dbContext.File.AsNoTracking()
                                       where f.FilePostOwnerId == ent.PostId
                                       orderby f.FileUploadTime
                                       select f.FileUrl).ToListAsync();
                List<string> lists = new List<string>
                {

                };

                lists.AddRange(listImage);

                ent.PostImages = lists;
                ent.PostCommentCount = await (from c in _dbContext.Comment.AsNoTracking()
                                              where c.CommentPostId == ent.PostId
                                              && c.CommentIsDeleted == false
                                              select c.CommentId).CountAsync();
                ent.ShowDelete = team.TeamLeaderId == parameter.UserId || parameter.UserId == ent.PostUserId ? true : false;
            }

            return new PagedResponse<PostResponse>(entityList, parameter.PageSize, await query.CountAsync());
        }

        public async Task<PagedResponse<PostResponse>> GetPostPagingUser(PostRequestParameter parameter)
        {
            bool advanced = false;
            if (parameter.FromDate != null || parameter.ToDate != null || !string.IsNullOrEmpty(parameter.GroupId) || !string.IsNullOrEmpty(parameter.PostUser))
                advanced = true;

            var teamList = from t in _dbContext.Team.AsNoTracking()
                           join par in _dbContext.Participation.AsNoTracking() on t.TeamId equals par.ParticipationTeamId
                           where par.ParticipationUserId == parameter.UserId && par.ParticipationIsDeleted == false
                           select new { par.ParticipationUserId, t.TeamId, t.TeamName };
            var test = await teamList.ToListAsync();
            /*    //danh sách team mà user join
                teamList = teamList.Where(x => x.Id == parameter.UserId);*/

            //bảng chứa toàn bộ thông tin các post của các team mà user tham gia
            var query = from p in _dbContext.Post
                        join listTeam in teamList on p.PostTeamId equals listTeam.TeamId
                        join u in _dbContext.User on p.PostUserId equals u.Id
                        where p.PostIsDeleted == false
                        orderby p.PostCreatedAt descending
                        select new { p, u, p.Comments.Count, p.PostTeamId, RCount = p.PostReacts.Count, listTeam.TeamName };


            //tìm kiếm nâng cao
            if (advanced)
            {
                if (parameter.FromDate != null)
                {
                    DateTime dtFrom = (DateTimeOffset.FromUnixTimeMilliseconds((long)parameter.FromDate)).UtcDateTime;
                    query = query.Where(x => x.p.PostCreatedAt >= dtFrom);
                }
                if (parameter.ToDate != null)
                {
                    DateTime dateTo = (DateTimeOffset.FromUnixTimeMilliseconds((long)parameter.ToDate)).UtcDateTime;
                    query = query.Where(x => x.p.PostCreatedAt <= dateTo);
                }
                if (parameter.GroupId != null)
                {
                    query = query.Where(x => x.p.PostTeamId == parameter.GroupId);
                }
                if (parameter.PostUser != null)
                {
                    query = query.Where(x => x.u.Id == parameter.PostUser);
                }
            }

            if (!string.IsNullOrEmpty(parameter.BasicFilter) && !advanced)
            {
                DateTime now = DateTime.UtcNow;
                DateTime baseDate = now.Date;

                switch (parameter.BasicFilter)
                {
                    case BasicFilter.Lastest:
                        query = query.OrderByDescending(x => x.p.PostCreatedAt);
                        break;
                    case BasicFilter.LastHour:
                        var lastHour = now.AddMinutes(-60);
                        query = query.Where(x => x.p.PostCreatedAt >= lastHour && x.p.PostCreatedAt <= now);
                        break;
                    case BasicFilter.Today:
                        query = query.Where(x => ((DateTime)x.p.PostCreatedAt).Date == baseDate);
                        break;
                    case BasicFilter.ThisWeek:
                        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                        query = from q in query
                                where q.p.PostCreatedAt >= thisWeekStart && q.p.PostCreatedAt <= thisWeekEnd
                                select q;
                        //query.Where(x => x.p.PostCreatedAt >= flagstart && x.p.PostCreatedAt <= flagend);
                        break;
                    case BasicFilter.ThisMonth:
                        var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                        var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        query = from q in query
                                where q.p.PostCreatedAt >= thisMonthStart && q.p.PostCreatedAt <= thisMonthEnd
                                select q;
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(parameter.BasicFilter) && !advanced)
                query = query.OrderByDescending(x => x.p.PostCreatedAt);

            var reactQuery = from q in query
                             from r in _dbContext.PostReact.Where(r => r.PostReactPostId == q.p.PostId && r.PostReactUserId == parameter.UserId).DefaultIfEmpty()
                             select new { q, isReacted = r.PostReactUserId };

            if (parameter.PostId != null)
                reactQuery = reactQuery.Where(x => x.q.p.PostId == parameter.PostId);

            var entityList = await reactQuery.Select(x => new PostResponse
            {
                PostId = x.q.p.PostId,
                PostUserId = x.q.p.PostUserId,
                PostTeamId = x.q.p.PostTeamId,
                PostContent = x.q.p.PostContent,
                PostCreatedAt = x.q.p.PostCreatedAt.FormatTime(),
                PostCommentCount = x.q.Count,
                PostIsDeleted = x.q.p.PostIsDeleted,
                UserName = x.q.u.FullName,
                UserAvatar = x.q.u.ImageUrl,
                PostReactCount = x.q.RCount,
                TeamName = x.q.TeamName,
                IsReacted = x.isReacted == null ? false : true,
            }).Skip(parameter.SkipItems).Take(parameter.PageSize).ToListAsync();


            foreach (var ent in entityList)
            {
                var listImage = await (from f in _dbContext.File.AsNoTracking()
                                       where f.FilePostOwnerId == ent.PostId
                                       orderby f.FileUploadTime
                                       select f.FileUrl).ToListAsync();
                List<string> lists = new List<string>();

                lists.AddRange(listImage);

                ent.PostImages = lists;

                var team = await _dbContext.Team.FindAsync(ent.PostTeamId);
                if (team.TeamLeaderId == parameter.UserId || ent.PostUserId == parameter.UserId)
                    ent.ShowDelete = true;

                ent.PostCommentCount = await (from c in _dbContext.Comment.AsNoTracking()
                                              where c.CommentPostId == ent.PostId
                                              && c.CommentIsDeleted == false
                                              select c.CommentId).CountAsync();
            }

            return new PagedResponse<PostResponse>(entityList, parameter.PageSize, await query.CountAsync());
        }

        public async Task<List<UserResponse>> SearchUser(string userId, string keyWord)
        {
            var teamList = from t in _dbContext.Team.AsNoTracking()
                           join par in _dbContext.Participation.AsNoTracking() on t.TeamId equals par.ParticipationTeamId
                           join u in _dbContext.User.AsNoTracking() on par.ParticipationUserId equals u.Id
                           where par.ParticipationIsDeleted == false
                           select new { u, t.TeamId, t.TeamName };

            //danh sách team mà user join
            teamList = teamList.Where(x => x.u.Id == userId).Distinct();

            var query = await (from listTeam in teamList
                               join p in _dbContext.Participation.AsNoTracking() on listTeam.TeamId equals p.ParticipationTeamId
                               join u in _dbContext.User.AsNoTracking() on p.ParticipationUserId equals u.Id
                               select u).Distinct().ToListAsync();

            keyWord = keyWord.UnsignUnicode();

            /*if (!string.IsNullOrEmpty(keyWord))
                query = query.Where(x => x.FullName.UnsignUnicode().Contains(keyWord)).ToList();*/

            var outPut = query.Select(x => new UserResponse
            {
                UserId = x.Id,

                UserFullname = x.FullName,

                UserImageUrl = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,

            }).ToList();

            return outPut;

        }
    }
}
