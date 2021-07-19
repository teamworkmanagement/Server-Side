using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Appointment;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IAppoinmentRespository
    {
        Task<bool> CreateAppointment(CreateAppointmentRequest appoinment);
        Task<bool> DeleteAppointment(string appoinmentId);
        Task<List<AppointmentResponse>> GetByTeam(string teamId);
        Task<List<AppointmentResponse>> GetByUser(string userId);
        Task<bool> UpdateAppointment(UpdateAppointmentRequest updateApointmentRequest);
    }
}
