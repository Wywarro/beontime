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
        private readonly IWorkdayRepository workdayRepo;
        private readonly IAttendanceRepository attendanceRepo;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public WorkdayController(IWorkdayRepository workdayRepo, IAttendanceRepository attendanceRepo,
            IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.workdayRepo = workdayRepo;
            this.attendanceRepo = attendanceRepo;
            this.dateTimeProvider = dateTimeProvider;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkdayListModel>>> GetAllWorkdays()
        {
            string userId = User.FindFirst("id")?.Value;
            try
            {
                var results = await workdayRepo.GetAllWorkdays(userId);

                WorkdayListModel[] models = mapper.Map<WorkdayListModel[]>(results);

                return models;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}", Name = "GetWorkdayById")]
        public async Task<ActionResult<WorkdayDetailModel>> GetWorkday(int id)
        {
            try
            {
                var entity = await workdayRepo.GetWorkday(id);
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
        public async Task<ActionResult<WorkdayDetailModel>> CreateWorkday(WorkdayCreateModel workdayModel)
        {
            string userId = User.FindFirst("id")?.Value;
            try
            {
                var existingWorkday = await workdayRepo.GetWorkday(workdayModel.Datestamp, userId);
                if (existingWorkday != null)
                {
                    return BadRequest("Workday already exists at provided date!");
                }

                var workday = mapper.Map<WorkdayCreateModel, Workday>(workdayModel);
                var now = dateTimeProvider.GetDateTimeNow();

                workday.CreatedOn = now;
                workday.UpdatedOn = now;
                workday.UserId = userId;

                await workdayRepo.CreateWorkday(workday);
                return CreatedAtRoute("GetWorkdayById", new { id = workday.Id }, mapper.Map<WorkdayDetailModel>(workday));
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

                var oldWorkday = await workdayRepo.GetWorkday(id);
                if (oldWorkday == null)
                    NotFound($"Could not find workday with provided id={id}");

                var workday = mapper.Map(model, oldWorkday);
                workday.UpdatedOn = dateTimeProvider.GetDateTimeNow();

                await workdayRepo.UpdateWorkday(workday);
                return mapper.Map<WorkdayDetailModel>(workday);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkday(int id)
        {
            try
            {
                var oldWorkday = await workdayRepo.GetWorkday(id);
                if (oldWorkday == null)
                    NotFound($"Could not find workday with provided id={id}");

                await workdayRepo.DeleteWorkday(id);
                return Ok($"Workday with id={id} has been deleted!");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("/attendance")]
        public async Task<ActionResult<WorkdayDetailModel>> CreateAttendance(AttendanceCreateModel attendanceModel)
        {
            string userId = User.FindFirst("id")?.Value;
            bool isManager = User.IsInRole(Policies.Admin) || User.IsInRole(Policies.Manager);
            try
            {
                var getParentWorkday = await workdayRepo.GetWorkday(attendanceModel.Timestamp.Date, userId);

                var attendance = mapper.Map<AttendanceCreateModel, Attendance>(attendanceModel);
                var now = dateTimeProvider.GetDateTimeNow();

                attendance.CreatedOn = now;
                attendance.UpdatedOn = now;
                attendance.UserId = userId;

                if (getParentWorkday != null)
                {
                    getParentWorkday.Attendances.Add(attendance);
                    await workdayRepo.UpdateWorkday(getParentWorkday);
                }
                else
                {
                    Workday newWorkday = new Workday()
                    {
                        CreatedOn = now,
                        UpdatedOn = now,
                        UserId = userId,
                        Datestamp = attendance.Timestamp.Date,
                        Verified = isManager,
                        Attendances = new List<Attendance>()
                        {
                            attendance
                        },
                    };
                    await workdayRepo.CreateWorkday(newWorkday);
                }

                return CreatedAtRoute("GetWorkdayById", new { id = getParentWorkday.Id }, mapper.Map<WorkdayDetailModel>(getParentWorkday));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
