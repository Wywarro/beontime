using Beontime.Application.Attendances.Responses;
using MediatR;

namespace Beontime.Application.Attendances.Query
{

    public sealed class GetAllTimeCardsQuery : IRequest<GetAllTimeCardsResponse>
    {
    }
}
