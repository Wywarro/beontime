﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beontime.WebApi.Controllers
{
    [Route("api/v1/attendances")]
    [Authorize]
    public class AttendanceController : ApiControllerBase
    { }
}