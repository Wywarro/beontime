using Beontime.Domain.Aggregates;
using System.Collections.Generic;
using System.Linq;

namespace Beontime.Application.Attendances.Responses
{

    public sealed class GetAllTimeCardsResponse
    {
        public IEnumerable<TimeCard> TimeCards { get; set; } = Enumerable.Empty<TimeCard>();
    }
}
