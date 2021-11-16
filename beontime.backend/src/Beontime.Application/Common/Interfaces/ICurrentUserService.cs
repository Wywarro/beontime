using System;

namespace Beontime.Application.Common.Interfaces
{

    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
