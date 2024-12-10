using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManagementSystem.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        string signInUrl = "/Account/Login";
        List<string> passUrlList = new List<string>
        {
            "/Account/Login",
            "/Account/Register",
        };
        public async Task InvokeAsync(HttpContext context)
        {
            string url = context.Request.Path;
            if (passUrlList.Count(x =>
                x.ToLower() == url.ToLower()) > 0 ||
                url.ToLower() == signInUrl.ToLower())
                goto Result;

            var jwtToken = context.Request.Cookies["jwt_token"];
            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                context.Response.Redirect(signInUrl);
                goto Result;
            }

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(jwtToken);
       

            var item = decodedToken.Claims.FirstOrDefault(x => x.Type == "SessionExpired");
            DateTime tokenSessionExpired = Convert.ToDateTime(item?.Value);
            if (item is null || DateTime.Now > tokenSessionExpired)
            {
                context.Response.Redirect(signInUrl);
                goto Result;
            }

            var itemUsername = decodedToken.Claims.FirstOrDefault(x => x.Type == "email");
            if (itemUsername is null)
            {
                context.Response.Redirect(signInUrl);
                goto Result;
            }

            if (_configuration == null) goto Result;

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim("SessionExpired", DateTime.UtcNow.AddMinutes(30).ToString("o")),
                        new Claim(JwtRegisteredClaimNames.Email, itemUsername.Value),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            jwtToken = tokenHandler.WriteToken(token);

            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(1);

            context.Response.Cookies.Append("jwt_token", jwtToken, options);

            Result:
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    public static class JwtTokenMiddlewareExtension
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TokenMiddleware>();
        }
    }

}
