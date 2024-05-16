using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegistroActividades.Helpers
{
    public class JwtHelper
    {

        private readonly IConfiguration configuration;

        public JwtHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetToken(string username, string role, List<Claim> claims)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var issuer = configuration.GetSection("JwtBearer").GetValue<string>("Issuer");
            var audience = configuration.GetSection("JwtBearer").GetValue<string>("Audience");
            var secret = configuration.GetSection("JwtBearer").GetValue<string>("Secret");

            List<Claim> basicas = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience)
            };

            basicas.AddRange(claims);

            JwtSecurityToken jwt = new JwtSecurityToken(issuer, audience, basicas,DateTime.Now, DateTime.Now.AddMinutes(50), new SigningCredentials
                (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")), SecurityAlgorithms.Sha512));
            return handler.WriteToken(jwt);
        }
    }
}
