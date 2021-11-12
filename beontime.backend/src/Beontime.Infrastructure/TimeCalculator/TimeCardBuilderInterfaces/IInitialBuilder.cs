namespace Beontime.Infrastructure.TimeCalculator.TimeCardBuilderInterfaces
{
    using Beontime.Domain.Aggregates;
    using Beontime.Domain.Entities;

    public interface IInitialBuilder
    {
        IBreakWorkCalculator BuildInitialData(TimeCard timecard);
    }
}