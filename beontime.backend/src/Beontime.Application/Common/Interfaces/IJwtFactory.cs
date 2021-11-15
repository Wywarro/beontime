using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Beontime.Application.Common.Interfaces
{

    public interface IJwtFactory
    {
        ClaimsIdentity GenerateClaimsIdentity(string username, string id, IList<string> roles);
        Task<string> GenerateEncodedToken(string username, ClaimsIdentity identity);
    }
}
