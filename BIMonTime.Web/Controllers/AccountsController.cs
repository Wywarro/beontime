using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;
using BIMonTime.Services.Auth;
using BIMonTime.Services.DateTimeProvider;
using BIMonTime.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BIMonTime.Web.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserManager<BeOnTimeUser> userManager;
        private readonly IJwtFactory jwtFactory;

        public AccountsController(
            IMapper mapper,
            UserManager<BeOnTimeUser> userManager,
            IJwtFactory jwtFactory)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.jwtFactory = jwtFactory;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel registerModel)
        {
            var userIdentity = mapper.Map<BeOnTimeUser>(registerModel);
            var result = await userManager.CreateAsync(userIdentity, registerModel.Password);
            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(ModelState.AddErrorsToModelState(result));
            }

            return Ok("Account created!");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] CredentialsViewModel loginModel)
        {
            var identity = await GetClaimsIdentity(loginModel.Username, loginModel.Password);
            if (identity == null)
            {
                return BadRequest(ModelState.AddErrorToModelState("login_failure", "Invalid username or password."));
            }

            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(loginModel.Username, identity),
                expires_in = TimeSpan.FromMinutes(120)
            };

            return Ok(response);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return await NullClaimsIdentity();

            // get the user to verifty
            var userToVerify = await userManager.FindByNameAsync(username);

            if (userToVerify == null)
                return await NullClaimsIdentity();

            // check the credentials
            if (await userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(
                    jwtFactory.GenerateClaimsIdentity(username, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await NullClaimsIdentity();
        }

        private Task<ClaimsIdentity> NullClaimsIdentity() =>
            Task.FromResult<ClaimsIdentity>(null);
    }
}
