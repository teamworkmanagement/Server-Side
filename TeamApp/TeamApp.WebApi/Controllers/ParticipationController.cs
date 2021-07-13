using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Paricipation;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/participation")]
    public class ParticipationController : ControllerBase
    {
        private readonly IParticipationRepository _repo;
        public ParticipationController(IParticipationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Leave team API
        /// </summary>
        /// <param name="participationDeleteRequest"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteParticipation([FromQuery] ParticipationDeleteRequest participationDeleteRequest)
        {
            var res = await _repo.DeleteParticipation(participationDeleteRequest);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Add member of team API
        /// </summary>
        /// <param name="participationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(ApiResponse<ParticipationResponse>))]
        public async Task<IActionResult> AddParticipation(ParticipationRequest participationRequest)
        {
            var outPut = await _repo.AddParticipation(participationRequest);
            return Ok(new ApiResponse<ParticipationResponse>
            {
                Data = outPut,
                Succeeded = outPut == null ? false : true,
            });
        }
    }
}
