using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;
using BIMonTime.Services.DateTimeProvider;
using BIMonTime.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BIMonTime.Web.Controllers
{
    [Route("api/v1/workday")]
    [ApiController]
    [Authorize(Policy = "Employee")]
    public class WorkdayController : ControllerBase
    {
        private readonly IWorkdayRepository repository;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public WorkdayController(IWorkdayRepository repository, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.repository = repository;
            this.dateTimeProvider = dateTimeProvider;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkdayDetailModel>>> GetAllWorkdays()
        {
            string userId = User.FindFirst("id")?.Value;
            try
            {
                var results = await repository.GetAllWorkdays(userId);

                WorkdayDetailModel[] models = mapper.Map<WorkdayDetailModel[]>(results);

                return models;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkdayDetailModel>> Get(int id)
        {
            try
            {
                var entity = await repository.GetWorkday(id);
                if (entity == null) 
                    return NotFound();

                WorkdayDetailModel model = mapper.Map<WorkdayDetailModel>(entity);

                return model;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<WorkdayDetailModel>> Create(WorkdayDetailModel workdayModel)
        {
            string userId = User.FindFirst("id")?.Value;
            try
            {
                var existingWorkday = await repository.GetWorkday(workdayModel.Datestamp, userId);
                if (existingWorkday != null)
                {
                    return BadRequest("Workday already exists at provided date!");
                }

                var workday = mapper.Map<WorkdayDetailModel, Workday>(workdayModel);
                var now = dateTimeProvider.GetDateTimeNow();

                workday.CreatedOn = now;
                workday.UpdatedOn = now;
                workday.UserId = userId;

                await repository.CreateWorkday(workday);
                return CreatedAtAction("Get", new { id = workday.Id }, mapper.Map<WorkdayDetailModel>(workday));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        [HttpPut("{id}")]
        public IActionResult UpdateWorkday(int id, WorkdayDetailModel model)
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
