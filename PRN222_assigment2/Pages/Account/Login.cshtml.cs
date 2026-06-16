using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;
using PRN222_assigment2.Models;

namespace PRN222_assigment2.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectByUserRole();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userService.AuthenticateAsync(Input.Email, Input.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác. Vui lòng thử lại.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.Role == "Student" && await _userService.IsDefaultPasswordAsync(user.UserId))
            {
                TempData["Warning"] = "Bạn đang sử dụng mật khẩu mặc định. Vui lòng đổi mật khẩu để tiếp tục.";
                return RedirectToPage("/Profile/Index");
            }

            return RedirectByUserRole();
        }

        private IActionResult RedirectByUserRole()
        {
            if (User.IsInRole("Admin"))
                return RedirectToPage("/Account/Users");
            else if (User.IsInRole("Teacher"))
                return RedirectToPage("/Document/Index");
            else
                return RedirectToPage("/Chat/Index");
        }
    }
}
