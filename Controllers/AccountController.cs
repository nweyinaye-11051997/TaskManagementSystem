using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModel;

namespace TaskManagementSystem.Controllers
{
    
    public class AccountController : Controller
    {

        private readonly SignInManager<User> _siginManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public AccountController(SignInManager<User> siginManager, UserManager<User> userManager, IConfiguration configuration)
        {
            _siginManager = siginManager;
            _userManager = userManager;
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM view)
        {
            if (ModelState.IsValid)
            {
               var rs = await _siginManager.PasswordSignInAsync(view.Username, view.Password, view.RememberMe, false);
                if (rs.Succeeded)
                {
                    SetCookies(view);
                    return RedirectToAction("List", "Task");
                }
                ModelState.AddModelError("", "Invalid username or password!");
            }
            return View(view);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult> Register(RegisterVM view,string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    Name = view.Name,
                    UserName = view.Email,
                    Email = view.Email,
                };

                var result = await _userManager.CreateAsync(user, view.Password!);

                if (result.Succeeded)
                {
                    await _siginManager.SignInAsync(user, false);

                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(view);
        }

        public async Task<IActionResult> Logout()
        {
            await _siginManager.SignOutAsync();
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("Username");
            HttpContext.Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Login", "Account");
        }

        public void SetCookies(LoginVM view)
        {
            HttpContext.Session.SetString("Username", view.Username);

            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(1);
            HttpContext.Response.Cookies.Append("Username", view.Username, options);

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim("SessionExpired", DateTime.Now.AddMinutes(30).ToString("o")),
                        new Claim(JwtRegisteredClaimNames.Email, view.Username),
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
            var jwtToken = tokenHandler.WriteToken(token);
            HttpContext.Response.Headers.Append("jwt_token", jwtToken);
            HttpContext.Response.Cookies.Append("jwt_token", jwtToken, options);
            HttpContext.Session.SetString("jwt_token", jwtToken);
        }
    }
}
