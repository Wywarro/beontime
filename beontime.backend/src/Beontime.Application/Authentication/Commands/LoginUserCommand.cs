using Beontime.Application.Authentication.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Beontime.Application.Authentication.Commands
{

    public sealed class LoginUserCommand : IRequest<LoginTokenResponse>
    {
        [Required(ErrorMessage = "Email cannot be empty!")]
        public string Email { get; set; } = "";
    }
}
