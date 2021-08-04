namespace Beontime.WebApi.Controllers
{
    using System;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    [ApiController]
    [Route("[controller]")]
    public class ApiControllerBase : ControllerBase
    {
        private ISender? mediator;

        protected ISender Mediator => (mediator ??= HttpContext.RequestServices.GetService<ISender>())
                                      ?? throw new ArgumentNullException();
    }
}