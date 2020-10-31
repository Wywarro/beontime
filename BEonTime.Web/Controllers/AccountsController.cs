using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AspNetCore.Identity.Mongo.Model;
using AutoMapper;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;
using BEonTime.Services;
using BEonTime.Services.Auth;
using BEonTime.Services.EmailSender;
using BEonTime.Web.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BEonTime.Web.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserManager<BEonTimeUser> userManager;
        private readonly RoleManager<MongoRole> roleManager;
        private readonly IJwtFactory jwtFactory;
        private readonly IEmailSender emailSender;
        private readonly ILogger<AccountsController> logger;

        public AccountsController(
            IMapper mapper,
            UserManager<BEonTimeUser> userManager,
            RoleManager<MongoRole> roleManager,
            IJwtFactory jwtFactory, 
            IEmailSender emailSender,
            ILogger<AccountsController> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtFactory = jwtFactory;
            this.emailSender = emailSender;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterWithInvitation([FromBody] RegistrationViewModel registerModel)
        {
            var user = mapper.Map<BEonTimeUser>(registerModel);

            await CreateRoles();

            string randomPassword = $"{Password.Generate(32, 12)}11";
            var createUserResult = await userManager.CreateAsync(user, randomPassword.ToString());
            if (!createUserResult.Succeeded)
            {
                return new BadRequestObjectResult(ModelState.AddErrorsToModelState(createUserResult));
            }

            var addRoleToUserResult = await userManager.AddToRoleAsync(user, registerModel.Role);
            if (!addRoleToUserResult.Succeeded)
            {
                return new BadRequestObjectResult(ModelState.AddErrorsToModelState(addRoleToUserResult));
            }

            logger.LogInformation($"Generating code for password reset for {user.Id}");
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            var jobId = BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"You have been invited to BEonTime! " +
                $"Please start with resetting your password " +
                $"by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.")
            );

            return Ok("Account created!");
        }

        public async Task GeneratePasswordResetAndSendMailAsync(string email, string callbackUrl)
        {
            await emailSender.SendEmailAsync(
                email,
                "Reset Password",
                $"You have been invited to BEonTime! " +
                $"Please start with resetting your password " +
                $"by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            );
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
                id = identity.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value,
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
                var role = new MongoRole
                {
                    Name = "Admin"
                };
                await roleManager.CreateAsync(role);
            }

            x = await roleManager.RoleExistsAsync("Manager");
            if (!x)
            {
                var role = new MongoRole
                {
                    Name = "Manager"
                };
                await roleManager.CreateAsync(role);
            }

            x = await roleManager.RoleExistsAsync("Employee");
            if (!x)
            {
                var role = new MongoRole
                {
                    Name = "Employee"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
