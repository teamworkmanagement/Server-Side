using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Appoinment;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IAppoinmentRespository
    {
        Task<bool> CreateAppointment(CreateAppointmentRequest appoinment);
        Task<bool> DeleteAppointment(string appoinmentId);
        Task<List<AppointmentResponse>> GetByTeam(string teamId);
    }
}
