using Beontime.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Beontime.Infrastructure.JwtService
{

    public class JwtFactory : IJwtFactory
    {
        private readonly JwtConfigSettings jwtConfig;
        private readonly IDateTimeService dateTimeProvider;

        public JwtFactory(IOptions<JwtConfigSettings> jwtConfig, IDateTimeService dateTimeProvider)
        {
            this.jwtConfig = jwtConfig.Value;
            this.dateTimeProvider = dateTimeProvider;
        }

        public ClaimsIdentity GenerateClaimsIdentity(string username, string id, IList<string> roles)
        {
            return new ClaimsIdentity(new GenericIdentity(username, "Token"), new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, string.Join( ",", roles)),
            });
        }

        public async Task<string> GenerateEncodedToken(string username, ClaimsIdentity identity)
        {
            var now = dateTimeProvider.GetDateTimeNow();

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, username),
                 new Claim(JwtRegisteredClaimNames.Jti, await JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(ClaimTypes.NameIdentifier),
                 identity.FindFirst(ClaimTypes.Role)
             };

            var jwt = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: dateTimeProvider.GetDateTimeNow().AddMinutes(60),
                signingCredentials: SigningCredentials(jwtConfig));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public static Func<Task<string>> JtiGenerator =>
          () => Task.FromResult(Guid.NewGuid().ToString());

        private static SigningCredentials SigningCredentials(JwtConfigSettings jwtConfig)
        {
            var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.SecurityKey));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }
    }
}
