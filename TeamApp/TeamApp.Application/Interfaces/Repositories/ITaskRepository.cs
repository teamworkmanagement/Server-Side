﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Task;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskResponse>> GetAllByUserId(string userId);
        Task<List<TaskResponse>> GetAllByTeamId(string teamId);
        Task<List<TaskResponse>> GetAllByUserTeamId(string userId, string teamId);
        Task<PagedResponse<TaskResponse>> GetPaging(RequestParameter parameter);
        Task<string> AddTask(TaskRequest taskReq);
        Task<bool> UpdateTask(string taskId, TaskRequest taskReq);
        Task<bool> DeleteTask(string taskId);
    }
}