namespace Beontime.Infrastructure.TimeCalculator.TimeCardBuilderInterfaces
{
    public interface IBreakWorkCalculator
    {
        IWorkDurationCalculator CalculateBreakDuration();
    }
}