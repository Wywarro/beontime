using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BIMonTime.Services.DateTimeProvider;
using BIMonTime.Services.EnvironmentVariables;
using BIMonTime.Data.Models;
using System.Collections.Generic;

namespace BIMonTime.Services.Auth
{
    public interface IJwtFactory
    {
        ClaimsIdentity GenerateClaimsIdentity(string username, string id, IList<string> roles);
        Task<string> GenerateEncodedToken(string username, ClaimsIdentity identity);
    }

    public class JwtFactory : IJwtFactory
    {
        private readonly IConfiguration config;
        private readonly IDateTimeProvider dateTimeProvider;

        public JwtFactory(IConfiguration config, IDateTimeProvider dateTimeProvider)
        {
            this.config = config;
            this.dateTimeProvider = dateTimeProvider;
        }

        public ClaimsIdentity GenerateClaimsIdentity(string username, string id, IList<string> roles)
        {
            return new ClaimsIdentity(new GenericIdentity(username, "Token"), new[]
            {
                new Claim("id", id),
                new Claim("role", string.Join( ",", roles)),
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
                 identity.FindFirst("id"),
                 identity.FindFirst("role")
             };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: dateTimeProvider.GetDateTimeNow().AddMinutes(60),
                signingCredentials: SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public Func<Task<string>> JtiGenerator =>
          () => Task.FromResult(Guid.NewGuid().ToString());

        private SigningCredentials SigningCredentials 
        {
            get
            {
                var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(EnvVarProvider.SecretKey));
                return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            }
        }
    }
}
