namespace Beontime.Infrastructure.TimeCalculator.TimeCardBuilderInterfaces
{
    using Domain.Entities;

    public interface IInitialBuilder
    {
        IBreakWorkCalculator BuildInitialData(WorkdayEntity workday);
    }
}