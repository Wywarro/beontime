using Beontime.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.Application.Common.Behaviors
{

    public sealed class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : class
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Webinar202103 Request: {Name} {@UserId} {@Request}",
                requestName, userId, request);

            return Task.CompletedTask;
        }
    }
}
