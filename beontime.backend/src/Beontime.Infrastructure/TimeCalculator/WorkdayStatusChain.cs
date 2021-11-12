using Beontime.Domain.Aggregates;
using Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers;
using System;

namespace Beontime.Infrastructure.TimeCalculator
{
    internal static class WorkdayStatusChain
    {
        public static GenericStatusChainHandler GenerateStatus(TimeCard timeCard, DateTime now)
        {
            var immutableStatusHandler = new ImmutableStatusChainHandler(timeCard, now);
            var unexcusedAbsenceHandler = new UnexcusedAbsenceChainHandler(timeCard, now);
            var notYetAtWorkHandler = new NotYetAtWorkChainHandler(timeCard, now);
            var todayInvalidLogsHandler = new TodayInvalidLogsStatusHandler(timeCard, now);
            var presentHandler = new PresentChainHandler(timeCard, now);
            var breakHandler = new BreakChainHandler(timeCard, now);
            var notTodayInvalidLogsHandler = new NotTodayInvalidLogsChainHandler(timeCard, now);
            var fulltimeHandler = new FulltimeDayChainHandler(timeCard, now);
            var undertimeHandler = new UndertimeDayChainHandler(timeCard, now);
            var overtimeHandler = new OvertimeDayChainHandler(timeCard, now);
            var invalidStatusHandler = new FallbackStatusChainHandler(timeCard, now);

            var firstHandler = immutableStatusHandler;

            firstHandler
                .SetNext(unexcusedAbsenceHandler)
                .SetNext(notYetAtWorkHandler)
                .SetNext(todayInvalidLogsHandler)
                .SetNext(presentHandler)
                .SetNext(breakHandler)
                .SetNext(notTodayInvalidLogsHandler)
                .SetNext(fulltimeHandler)
                .SetNext(undertimeHandler)
                .SetNext(overtimeHandler)
                .SetNext(invalidStatusHandler);

            return firstHandler;
        }
    }
}
