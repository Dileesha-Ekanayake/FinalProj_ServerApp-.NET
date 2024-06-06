using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServerApp.Security
{
    public class JwtTokenUtil
    {
        private readonly string _key;
        private readonly UserService _userService;

        public JwtTokenUtil(UserService userService)
        {
            _userService = userService;
        }

        public JwtTokenUtil(string key)
        {
            _key = key;
        }

        public string GenerateToken(IUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes("Jwt:Key");
            var securityKey = new SymmetricSecurityKey(keyBytes);


            var claims = user.GenerateClaims().Select(claim => claim.Value).Select(value => new Claim("aud", value));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("user", user.UserName),
                }.Concat(claims)),
                Expires = DateTime.UtcNow.AddMinutes(5),
                //SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string[] GetAuthoritiesFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.Payload.Aud.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting authorities from token: " + ex.Message);
                return null;
            }
        }

    }
}
