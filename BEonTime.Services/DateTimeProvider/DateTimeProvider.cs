using System;

namespace BIMonTime.Services.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime GetDateTimeNow();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }
    }
}
