namespace Beontime.Infrastructure.TimeCalculator
{
    using Domain.Aggregates;

    public interface ITimeCardBuilder
    {
        TimeCard GetTimeCard();
    }
}