using Beontime.Domain.Aggregates;
using Beontime.Domain.Entities;
using Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers;
using System;

namespace Beontime.Infrastructure.TimeCalculator
{
    internal static class WorkdayStatusChain
    {
        public static GenericStatusChainHandler GenerateStatus(TimeCard timeCard, DateTime now)
        {
            var unexcusedAbsenceHandler = new UnexcusedAbsenceChainHandler(timeCard, now);
            var notYetAtWorkHandler = new NotYetAtWorkChainHandler(timeCard, now);
            var todayInvalidLogsHandler = new TodayInvalidLogsStatusHandler(timeCard, now);
            var presentHandler = new PresentChainHandler(timeCard, now);
            var breakHandler = new BreakChainHandler(timeCard, now);
            var notTodayInvalidLogsHandler = new NotTodayInvalidLogsChainHandler(timeCard, now);
            var invalidStatusHandler = new FallbackStatusChainHandler(timeCard, now);

            unexcusedAbsenceHandler
                .SetNext(notYetAtWorkHandler)
                .SetNext(todayInvalidLogsHandler)
                .SetNext(presentHandler)
                .SetNext(breakHandler)
                .SetNext(notTodayInvalidLogsHandler)
                .SetNext(invalidStatusHandler);

            return unexcusedAbsenceHandler;
        }
    }
}
