using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;
using BEonTime.Services.DateTimeProvider;
using BEonTime.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BEonTime.Web.Controllers
{
    [Route("api/v1/workday")]
    [ApiController]
    [Authorize(Policy = "Employee")]
    public class WorkdayController : ControllerBase
    {
        private readonly IWorkdayRepository workdayRepo;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public WorkdayController(IWorkdayRepository workdayRepo,
            IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.workdayRepo = workdayRepo;
            this.dateTimeProvider = dateTimeProvider;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkdayListModel>>> GetAllWorkdays()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var results = await workdayRepo.FilterByAsync(
                workday => workday.UserId == userId);

            WorkdayListModel[] models = mapper.Map<WorkdayListModel[]>(results);

            return models;
        }

        [HttpGet("{id}", Name = "GetWorkdayById")]
        public async Task<ActionResult<WorkdayDetailModel>> GetWorkday(string id)
        {
            var entity = await workdayRepo.FindByIdAsync(id);
            if (entity == null)
                return NotFound($"Could not find workday with provided id={id}");

            WorkdayDetailModel model = mapper.Map<WorkdayDetailModel>(entity);

            return model;
        }

        [HttpPost]
        public async Task<ActionResult<WorkdayDetailModel>> CreateWorkday(WorkdayCreateModel workdayModel)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var existingWorkday = await workdayRepo.FilterByAsync(workday => 
                workday.Datestamp == workdayModel.Datestamp &&
                workday.UserId == userId);
            if (existingWorkday != null)
                return BadRequest("Workday already exists at provided date!");

            var workday = mapper.Map<WorkdayCreateModel, Workday>(workdayModel);
            var now = dateTimeProvider.GetDateTimeNow();

            workday.UpdatedOn = now;
            workday.UserId = userId;

            await workdayRepo.InsertOneAsync(workday);
            return CreatedAtRoute("GetWorkdayById", new { id = workday.Id }, mapper.Map<WorkdayDetailModel>(workday));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkdayDetailModel>> UpdateWorkday(string id, WorkdayUpdateModel model)
        {
            if (id != model.Id)
                return BadRequest("Id of workday in payload and Id in request doesn't match!");

            var oldWorkday = await workdayRepo.FindByIdAsync(id);
            if (oldWorkday == null)
                return NotFound($"Could not find workday with provided id={id}");

            var workday = mapper.Map(model, oldWorkday);
            workday.UpdatedOn = dateTimeProvider.GetDateTimeNow();

            await workdayRepo.ReplaceOneAsync(workday);
            return mapper.Map<WorkdayDetailModel>(workday);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkday(string id)
        {
            var oldWorkday = await workdayRepo.FindByIdAsync(id);
            if (oldWorkday == null)
                return NotFound($"Could not find workday with provided id={id}");

            await workdayRepo.DeleteByIdAsync(id);
            return Ok($"Workday with id={id} has been deleted!");
        }

        [HttpPost("/attendance")]
        public async Task<ActionResult<WorkdayDetailModel>> CreateAttendance(AttendanceCreateModel attendanceModel)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool isManager = User.IsInRole(Policies.Admin) || User.IsInRole(Policies.Manager);
            var getParentWorkday = await workdayRepo.FindOneAsync(workday =>
                workday.Datestamp == attendanceModel.Timestamp.Date &&
                workday.UserId == userId);
            var attendance = mapper.Map<AttendanceCreateModel, Attendance>(attendanceModel);
            var now = dateTimeProvider.GetDateTimeNow();

            attendance.UpdatedOn = now;

            if (getParentWorkday != null)
            {
                getParentWorkday.Attendances.Add(attendance);
                await workdayRepo.ReplaceOneAsync(getParentWorkday);
            }
            else
            {
                Workday newWorkday = new Workday()
                {
                    UpdatedOn = now,
                    UserId = userId,
                    Datestamp = attendance.Timestamp.Date,
                    Verified = isManager,
                    Attendances = new List<Attendance>()
                    {
                        attendance
                    },
                };
                await workdayRepo.InsertOneAsync(newWorkday);
            }

            return CreatedAtRoute("GetWorkdayById", 
                new { id = getParentWorkday.Id }, 
                mapper.Map<WorkdayDetailModel>(getParentWorkday));
        }
    }
}
