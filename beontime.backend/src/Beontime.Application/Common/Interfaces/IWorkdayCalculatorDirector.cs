namespace Beontime.Application.Common.Interfaces
{
    using Domain.Entities;

    public interface IWorkdayCalculatorDirector
    {
        WorkdayEntity BuildWorkday();
    }
}