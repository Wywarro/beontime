using System;

namespace BIMonTime.Services.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime GetDateTimeNow();
        DateTime GetDateTimeInHourFromNow();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        public DateTime GetDateTimeInHourFromNow()
        {
            return DateTime.Now.AddMinutes(60);
        }
    }
}
