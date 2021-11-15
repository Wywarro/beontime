namespace Beontime.Infrastructure.Services
{
    using System;
    using Application.Common.Interfaces;

    public class DateTimeService : IDateTimeService
    {
        public DateTime GetDateTimeNow() => DateTime.Now;
    }
}