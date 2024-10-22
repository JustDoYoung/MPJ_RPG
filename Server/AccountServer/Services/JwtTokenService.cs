
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountServer.Services
{
    public class JwtTokenService
    {
        const string SECRET_KEY = "012345678901234567890123456789ab"; // TODO

        IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateJwtAccessToken(long accountDbId)
        {
            long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            long expired = now + 3600 * 24 * 1000;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, accountDbId.ToString()), // Subject
                new Claim(JwtRegisteredClaimNames.Name, "Scott"), //temp Name
				new Claim(JwtRegisteredClaimNames.Iat, now.ToString()), // Issued At
				new Claim(JwtRegisteredClaimNames.Exp, expired.ToString()) // Expiration
			};

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY)),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(claims: claims, signingCredentials: credentials);
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            // TEST
            {
                var test1 = DecipherJwtAccessToken(token);
                var test2 = ValidateJwtAccessToken(token, SECRET_KEY);
                var test3 = ValidateJwtAccessToken(token, "abcdabcdabcdab12");
            }

            return token;
        }

        public JwtSecurityToken DecipherJwtAccessToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decipher = handler.ReadJwtToken(token);
            return decipher;
        }

        public int GetAccountDbIdFromToken(string token)
        {
            JwtSecurityToken jwtToken = DecipherJwtAccessToken(token);
            string? value = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (value == null) return 0;

            return int.Parse(value);
        }

        public string? GetUsernameFromToken(string token)
        {
            JwtSecurityToken jwtToken = DecipherJwtAccessToken(token);
            string? value = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;

            return value;
        }

        public bool ValidateJwtAccessToken(string token)
        {
            return ValidateJwtAccessToken(token, SECRET_KEY);
        }

        public bool ValidateJwtAccessToken(string token, string key)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParams = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            SecurityToken validatedToken;
            try
            {
                var claims = handler.ValidateToken(token, validationParams, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
