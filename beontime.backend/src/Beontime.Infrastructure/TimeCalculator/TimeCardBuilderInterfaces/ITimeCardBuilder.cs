using Beontime.Domain.Aggregates;

namespace Beontime.Infrastructure.TimeCalculator
{
    public interface ITimeCardBuilder
    {
        TimeCard GetTimeCard();
    }
}