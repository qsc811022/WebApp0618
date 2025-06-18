

using Dapper;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Security.Claims;

using WebApp.Models;

namespace WebApp.Controllers
{
    public class AccountController:Controller
    {
        private readonly IDbConnection _db;

        public AccountController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 檢查 Email 是否已存在
            var exists = await _db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Email = @Email",
                new { model.Email });

            if (exists > 0)
            {
                ModelState.AddModelError("Email", "此 Email 已被註冊");
                return View(model);
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var sql = @"
                INSERT INTO Users (Name, Email, PasswordHash, Role, Phone, Gender)
                VALUES (@Name, @Email, @PasswordHash, 'Student', @Phone, @Gender);
            ";

            await _db.ExecuteAsync(sql, new
            {
                model.Name,
                model.Email,
                PasswordHash = passwordHash,
                model.Phone,
                model.Gender
            });

            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.QuerySingleOrDefaultAsync<dynamic>(
                "SELECT * FROM Users WHERE Email = @Email", new { model.Email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, (string)user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                return View(model);
            }

            // 寫入登入 Cookie
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Course");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Course");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
