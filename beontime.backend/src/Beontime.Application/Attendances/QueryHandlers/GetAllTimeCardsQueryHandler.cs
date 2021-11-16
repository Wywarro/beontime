using Beontime.Application.Attendances.Query;
using Beontime.Application.Attendances.Responses;
using Beontime.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.Application.Attendances.QueryHandlers
{

    public sealed class GetAllTimeCardsQueryHandler
        : IRequestHandler<GetAllTimeCardsQuery, GetAllTimeCardsResponse>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly ITimeCardsRepository timeCardsRepository;

        public GetAllTimeCardsQueryHandler(
            ICurrentUserService currentUserService,
            ITimeCardsRepository timeCardsRepository)
        {
            this.currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this.timeCardsRepository = timeCardsRepository ?? throw new ArgumentNullException(nameof(timeCardsRepository));
        }

        public async Task<GetAllTimeCardsResponse> Handle(
            GetAllTimeCardsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            var timeCards = await timeCardsRepository.GetAllTimeCards(userId, cancellationToken);

            var response = new GetAllTimeCardsResponse
            {
                TimeCards = timeCards,
            };

            return response;
        }
    }
}
