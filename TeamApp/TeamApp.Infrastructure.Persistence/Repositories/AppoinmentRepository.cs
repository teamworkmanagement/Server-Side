using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Appoinment;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class AppoinmentRepository : IAppoinmentRespository
    {
        private readonly TeamAppContext _dbContext;
        public AppoinmentRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateAppointment(CreateAppointmentRequest appoinmentReq)
        {
            var appoinment = new Appointment
            {
                Id = Guid.NewGuid().ToString(),
                Name = appoinmentReq.Name,
                UserCreateId = appoinmentReq.UserCreateId,
                Date = appoinmentReq.Date,
                Hour = appoinmentReq.Hour,
                Minute = appoinmentReq.Minute,
                Description = appoinmentReq.Description,
                Type = appoinmentReq.Type,
                TeamId = appoinmentReq.TeamId,
                CreatedAt = DateTime.UtcNow,
            };

            await _dbContext.AddAsync(appoinment);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAppointment(string appoinmentId)
        {
            var appointment = await (from ap in _dbContext.Appointment
                                     where ap.Id == appoinmentId
                                     select ap).FirstOrDefaultAsync();

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            _dbContext.Remove(appointment);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<AppointmentResponse>> GetByTeam(string teamId)
        {
            var appointments = await (from ap in _dbContext.Appointment.AsNoTracking()
                                      join u in _dbContext.User.AsNoTracking()
                                      on ap.UserCreateId equals u.Id
                                      where ap.TeamId == teamId
                                      orderby ap.CreatedAt descending
                                      select new { ap, u.FullName, u.ImageUrl }).ToListAsync();

            return appointments.Select(ap => new AppointmentResponse
            {
                Id = ap.ap.Id,
                UserCreateId = ap.ap.UserCreateId,
                UserCreateName = ap.FullName,
                UserCreateAvatar = ap.ImageUrl,
                Name = ap.ap.Name,
                Date = ap.ap.Date.FormatTime(),
                Hour = ap.ap.Hour,
                Minute = ap.ap.Minute,
                Description = ap.ap.Description,
                Type = ap.ap.Type,
                TeamId = ap.ap.TeamId,
            }).ToList();
        }
    }
}
