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
        public async Task<ActionResult<IEnumerable<WorkdayListModel>>> GetAllWorkdays()
        {
            string userId = User.FindFirst("id")?.Value;
            try
            {
                var results = await repository.GetAllWorkdays(userId);

                WorkdayListModel[] models = mapper.Map<WorkdayListModel[]>(results);

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
        public async Task<ActionResult<WorkdayDetailModel>> Create(WorkdayCreateModel workdayModel)
        {
            string userId = User.FindFirst("id")?.Value;
            try
            {
                var existingWorkday = await repository.GetWorkday(workdayModel.Datestamp, userId);
                if (existingWorkday != null)
                {
                    return BadRequest("Workday already exists at provided date!");
                }

                var workday = mapper.Map<WorkdayCreateModel, Workday>(workdayModel);
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
        public async Task<ActionResult<WorkdayDetailModel>> UpdateWorkday(int id, WorkdayUpdateModel model)
        {
            try
            {
                if (id != model.Id)
                    return BadRequest("Id of workday in payload and Id in request doesn't match!");

                var oldWorkday = await repository.GetWorkday(id);
                if (oldWorkday == null)
                    NotFound($"Could not find workday with provided id={id}");

                var workday = mapper.Map(model, oldWorkday);
                workday.UpdatedOn = dateTimeProvider.GetDateTimeNow();

                await repository.UpdateWorkday(workday);
                return mapper.Map<WorkdayDetailModel>(workday);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var oldWorkday = await repository.GetWorkday(id);
                if (oldWorkday == null)
                    NotFound($"Could not find workday with provided id={id}");

                await repository.DeleteWorkday(id);
                return Ok($"Workday with id={id} has been deleted!");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
