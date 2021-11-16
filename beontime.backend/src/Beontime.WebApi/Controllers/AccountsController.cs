using Beontime.Application.Authentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.WebApi.Controllers
{
    [Route("api/v1/accounts")]
    [Authorize]
    public class AccountsController : ApiControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterWithInvitation(
            [FromBody] LoginUserCommand command,
            CancellationToken ct)
        {
            await Mediator.Send(command, ct);

            return Ok("Account created!");
        }
    }
}
