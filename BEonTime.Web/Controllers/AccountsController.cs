using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;
using BEonTime.Services.Auth;
using BEonTime.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BEonTime.Web.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserManager<BEonTimeUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtFactory jwtFactory;

        public AccountsController(
            IMapper mapper,
            UserManager<BEonTimeUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtFactory jwtFactory)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtFactory = jwtFactory;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel registerModel)
        {
            var userIdentity = mapper.Map<BEonTimeUser>(registerModel);

            await CreateRoles();

            var createUserResult = await userManager.CreateAsync(userIdentity, registerModel.Password);
            if (!createUserResult.Succeeded)
            {
                return new BadRequestObjectResult(ModelState.AddErrorsToModelState(createUserResult));
            }

            var addRoleToUserResult = await userManager.AddToRoleAsync(userIdentity, "Employee");
            if (!addRoleToUserResult.Succeeded)
            {
                return new BadRequestObjectResult(ModelState.AddErrorsToModelState(createUserResult));
            }

            return Ok("Account created!");
        }

        [HttpPost("login")]
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
                var roles = await userManager.GetRolesAsync(userToVerify);
                return await Task.FromResult(
                    jwtFactory.GenerateClaimsIdentity(username, userToVerify.Id.ToString(), roles));
            }

            // Credentials are invalid, or account doesn't exist
            return await NullClaimsIdentity();
        }

        private Task<ClaimsIdentity> NullClaimsIdentity() =>
            Task.FromResult<ClaimsIdentity>(null);

        private async Task CreateRoles()
        {
            bool x = await roleManager.RoleExistsAsync("Admin");
            if (!x)
            {
                // first we create Admin rool    
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                await roleManager.CreateAsync(role);
            }

            x = await roleManager.RoleExistsAsync("Manager");
            if (!x)
            {
                var role = new IdentityRole
                {
                    Name = "Manager"
                };
                await roleManager.CreateAsync(role);
            }

            x = await roleManager.RoleExistsAsync("Employee");
            if (!x)
            {
                var role = new IdentityRole
                {
                    Name = "Employee"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
