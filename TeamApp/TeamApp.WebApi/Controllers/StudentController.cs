using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Infrastructure.Persistence.Contexts;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Entities;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public StudentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("/getall")]
        public async Task<IActionResult> GetAll()
        {
            var outPut = await _dbContext.Students.ToListAsync();
            return Ok(outPut);
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            var outPut = await _dbContext.Students.FindAsync(id);
            if (outPut == null)
                return BadRequest();
            var a = new Response<Student>
            {
                Succeeded = true,
                Data = outPut,
            };
            return Ok(outPut);
        }
    }
}
