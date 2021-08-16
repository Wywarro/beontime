namespace Beontime.Infrastructure.TimeCalculator.TimeCardBuilderInterfaces
{
    public interface IWorkDurationCalculator
    {
        ITimeCardBuilder CalculateWorkingDuration();
    }
}