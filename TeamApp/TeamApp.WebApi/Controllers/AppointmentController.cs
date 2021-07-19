using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Appointment;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/appointment")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppoinmentRespository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public AppointmentController(IAppoinmentRespository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest appoinment)
        {
            appoinment.UserCreateId = _authenticatedUserService.UserId;
            var outPut = await _repo.CreateAppointment(appoinment);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(string id)
        {
            var outPut = await _repo.DeleteAppointment(id);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetByTeam(string teamId)
        {
            var outPut = await _repo.GetByTeam(teamId);
            return Ok(new ApiResponse<List<AppointmentResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppointment([FromBody] UpdateAppointmentRequest updateAppointmentRequest)
        {
            var outPut = await _repo.UpdateAppointment(updateAppointmentRequest);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetByUser()
        {
            var outPut = await _repo.GetByUser(_authenticatedUserService.UserId);
            return Ok(new ApiResponse<List<AppointmentResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
    }
}
