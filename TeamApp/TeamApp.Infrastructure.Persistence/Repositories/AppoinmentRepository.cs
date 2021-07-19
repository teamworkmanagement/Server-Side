using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.Utils;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.App;
using TeamApp.Application.DTOs.Appointment;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class AppoinmentRepository : IAppoinmentRespository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubAppClient, IHubAppClient> _hubApp;
        public AppoinmentRepository(TeamAppContext dbContext, IHubContext<HubAppClient, IHubAppClient> hubApp)
        {
            _dbContext = dbContext;
            _hubApp = hubApp;
        }
        public async Task<bool> CreateAppointment(CreateAppointmentRequest appointmentReq)
        {
            var appoinment = new Appointment
            {
                Id = Guid.NewGuid().ToString(),
                Name = appointmentReq.Name,
                UserCreateId = appointmentReq.UserCreateId,
                Date = appointmentReq.Date,
                Description = appointmentReq.Description,
                Type = appointmentReq.Type,
                TeamId = appointmentReq.TeamId,
                CreatedAt = DateTime.UtcNow,
            };

            await _dbContext.AddAsync(appoinment);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join uc in _dbContext.UserConnection.AsNoTracking()
                                     on p.ParticipationUserId equals uc.UserId
                                     where p.ParticipationIsDeleted == false && p.ParticipationTeamId == appointmentReq.TeamId
                                     select uc.ConnectionId).ToListAsync();

                await _hubApp.Clients.Clients(clients).ReloadAppointment(new
                {
                    TeamId = appointmentReq.TeamId,
                });

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAppointment(string appoinmentId)
        {
            var appointment = await (from ap in _dbContext.Appointment
                                     where ap.Id == appoinmentId
                                     select ap).FirstOrDefaultAsync();

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            _dbContext.Remove(appointment);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join uc in _dbContext.UserConnection.AsNoTracking()
                                     on p.ParticipationUserId equals uc.UserId
                                     where p.ParticipationIsDeleted == false && p.ParticipationTeamId == appointment.TeamId
                                     select uc.ConnectionId).ToListAsync();

                await _hubApp.Clients.Clients(clients).ReloadAppointment(new
                {
                    TeamId = appointment.TeamId,
                });

                return true;
            }
            return false;
        }

        public async Task<List<AppointmentResponse>> GetByTeam(string teamId)
        {
            var appointments = await (from ap in _dbContext.Appointment.AsNoTracking()
                                      join u in _dbContext.User.AsNoTracking()
                                      on ap.UserCreateId equals u.Id
                                      where ap.TeamId == teamId && ap.Date >= DateTime.UtcNow
                                      orderby ap.CreatedAt descending
                                      select new { ap, u.FullName, u.ImageUrl }).ToListAsync();

            return appointments.Select(ap => new AppointmentResponse
            {
                Id = ap.ap.Id,
                UserCreateId = ap.ap.UserCreateId,
                UserCreateName = ap.FullName,
                UserCreateAvatar = string.IsNullOrEmpty(ap.ImageUrl) ? $"https://ui-avatars.com/api/?name={ap.FullName}" : ap.ImageUrl,
                Name = ap.ap.Name,
                Date = ap.ap.Date.FormatTime(),
                Description = ap.ap.Description,
                Type = ap.ap.Type,
                TeamId = ap.ap.TeamId,
            }).ToList();
        }

        public async Task<List<AppointmentResponse>> GetByUser(string userId)
        {
            var teams = await (from p in _dbContext.Participation.AsNoTracking()
                               where p.ParticipationIsDeleted == false && p.ParticipationUserId == userId
                               select p.ParticipationTeamId).Distinct().ToListAsync();

            var appointments = await (from ap in _dbContext.Appointment.AsNoTracking()
                                      join u in _dbContext.User.AsNoTracking() on ap.UserCreateId equals u.Id
                                      where teams.Contains(ap.TeamId) && ap.Date >= DateTime.UtcNow
                                      orderby ap.CreatedAt descending
                                      select new { ap, u.FullName, u.ImageUrl }).ToListAsync();

            return appointments.Select(ap => new AppointmentResponse
            {
                Id = ap.ap.Id,
                UserCreateId = ap.ap.UserCreateId,
                UserCreateName = ap.FullName,
                UserCreateAvatar = string.IsNullOrEmpty(ap.ImageUrl) ? $"https://ui-avatars.com/api/?name={ap.FullName}" : ap.ImageUrl,
                Name = ap.ap.Name,
                Date = ap.ap.Date.FormatTime(),
                Description = ap.ap.Description,
                Type = ap.ap.Type,
                TeamId = ap.ap.TeamId,
            }).ToList();
        }

        public async Task<bool> UpdateAppointment(UpdateAppointmentRequest updateApointmentRequest)
        {
            var appointment = await _dbContext.Appointment.Where(ap => ap.Id == updateApointmentRequest.Id)
                .FirstOrDefaultAsync();

            if (appointment == null)
                throw new KeyNotFoundException("Not found appointment");

            appointment.Name = updateApointmentRequest.Name;
            appointment.Description = updateApointmentRequest.Description;
            appointment.Date = updateApointmentRequest.Date;
            appointment.Type = updateApointmentRequest.Type;

            _dbContext.Update(appointment);

            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join uc in _dbContext.UserConnection.AsNoTracking()
                                     on p.ParticipationUserId equals uc.UserId
                                     where p.ParticipationIsDeleted == false && p.ParticipationTeamId == appointment.TeamId
                                     select uc.ConnectionId).ToListAsync();

                await _hubApp.Clients.Clients(clients).ReloadAppointment(new
                {
                    TeamId = appointment.TeamId,
                });

                return true;
            }
            return false;

        }
    }
}
