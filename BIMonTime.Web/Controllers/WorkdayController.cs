using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;
using BIMonTime.Services.DateTimeProvider;
using BIMonTime.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BIMonTime.Web.Controllers
{
    [Route("api/v1/workday")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
        private readonly IWorkdayRepository repository;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public WorkdayController(IWorkdayRepository repository, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkdayModel>>> GetAllWorkdays()
        {
            try
            {
                var results = await repository.GetAllWorkdays();

                WorkdayModel[] models = mapper.Map<WorkdayModel[]>(results);

                return models;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkdayModel>> Get(int id)
        {
            try
            {
                var entity = await repository.GetWorkday(id);
                if (entity == null) return NotFound();

                WorkdayModel model = mapper.Map<WorkdayModel>(entity);

                return model;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<WorkdayModel>> Create(WorkdayModel workdayModel)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var existingWorkday = await repository.GetWorkday(workdayModel.Datestamp, userId);

                var workday = mapper.Map<WorkdayModel, Workday>(workdayModel);
                var now = dateTimeProvider.GetDateTimeNow();
                workday.CreatedOn = now;
                workday.UpdatedOn = now;
                await repository.CreateWorkday(workday);
                return CreatedAtAction("Get", new { id = workday.Id }, mapper.Map<WorkdayModel>(workday));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        [HttpPut("{id}")]
        public IActionResult UpdateWorkday(int id, WorkdayModel model)
        {
            //if (id != model.Id) return BadRequest();

            //WorkdayModel updatedEntity = await repository.Update(model);
            //return Ok(updatedEntity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await repository.DeleteWorkday(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
