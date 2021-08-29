namespace Beontime.Infrastructure.TimeCalculator.TimeCardBuilderInterfaces
{
    public interface IWorkDurationCalculator
    {
        IStatusResolver CalculateWorkingDuration();
    }
}