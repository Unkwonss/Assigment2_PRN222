using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;
using PRN222_assigment2.Models;

namespace PRN222_assigment2.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new();

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

            var result = await _userService.RegisterAsync(
                Input.Username,
                Input.Password,
                Input.FullName,
                Input.Email,
                "Student");

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc Email đã tồn tại trong hệ thống.");
                return Page();
            }

            TempData["Success"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToPage("/Account/Login");
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
