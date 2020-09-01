using System;

namespace BEonTime.Services.DateTimeProvider
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
