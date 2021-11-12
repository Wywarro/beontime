using System;

namespace Beontime.Domain.Enums
{
    public class TimeCardStatus
    {
        protected int Value { get; }

        static TimeCardStatus()
        {
            InvalidStatus = new TimeCardStatus(-1);
            Present = new TimeCardStatus(0);
            NormalDay = new TimeCardStatus(2);
            Overtime = new TimeCardStatus(3);
            Undertime = new TimeCardStatus(4);
            NotAtWorkYet = new TimeCardStatus(7);
            Break = new TimeCardStatus(8);
            InvalidLogs = new TimeCardStatus(9);
            UnexcusedAbsence = new TimeCardStatus(10);
        }

        protected TimeCardStatus(int value)
        {
            Value = value;
        }

        public static TimeCardStatus InvalidStatus { get; }
        public static TimeCardStatus Present { get; }
        public static TimeCardStatus NormalDay { get; }
        public static TimeCardStatus Overtime { get; }
        public static TimeCardStatus Undertime { get; }
        public static TimeCardStatus NotAtWorkYet { get; }
        public static TimeCardStatus Break { get; }
        public static TimeCardStatus InvalidLogs { get; }
        public static TimeCardStatus UnexcusedAbsence { get; }

        public override string ToString()
        {
            var timeCardStatus = Value switch
            {
                -1 => nameof(InvalidStatus),
                0 => nameof(Present),
                2 => nameof(NormalDay),
                3 => nameof(Overtime),
                4 => nameof(Undertime),
                7 => nameof(NotAtWorkYet),
                8 => nameof(Break),
                9 => nameof(InvalidLogs),
                10 => nameof(UnexcusedAbsence),
                _ => throw new ArgumentOutOfRangeException(),
            };

            return timeCardStatus;
        }
    }
}