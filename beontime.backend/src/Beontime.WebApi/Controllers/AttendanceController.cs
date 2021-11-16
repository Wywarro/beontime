using Beontime.Application.Attendances.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.WebApi.Controllers
{
    [Route("api/v1/attendances")]
    [Authorize]
    public class AttendanceController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> RegisterWithInvitation(CancellationToken ct)
        {
            var query = new GetAllTimeCardsQuery
            {

            };
            await Mediator.Send(query, ct);

            return Ok("Account created!");
        }
    }
}