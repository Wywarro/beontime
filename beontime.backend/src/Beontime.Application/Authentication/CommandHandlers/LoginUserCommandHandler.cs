using Beontime.Application.Authentication.Commands;
using Beontime.Application.Authentication.Responses;
using Beontime.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.Application.Authentication.CommandHandlers
{

    public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginTokenResponse>
    {
        private readonly IJwtFactory jwtFactory;

        public LoginUserCommandHandler(
            IPasswordGenerator passwordGenerator,
            IJwtFactory jwtFactory,
            ILogger<LoginUserCommandHandler> logger)
        {
            this.jwtFactory = jwtFactory ?? throw new ArgumentNullException(nameof(jwtFactory));
        }

        public async Task<LoginTokenResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.NewGuid();
            var identity = jwtFactory.GenerateClaimsIdentity(
                request.Email,
                userId.ToString(),
                new List<string>{ "admin" });

            var response = new LoginTokenResponse
            {
                Id = identity.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value,
                AuthToken = await jwtFactory.GenerateEncodedToken(request.Email, identity),
                ExpiresIn = TimeSpan.FromMinutes(120).TotalMilliseconds
            };

            return response;
        }
    }
}
