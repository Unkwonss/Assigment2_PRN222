using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;

namespace PRN222_assigment2.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public BusinessLayer.DTOs.UserDto? CurrentUser { get; set; }
        public bool IsDefaultPassword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetCurrentUserId();
            CurrentUser = await _userService.GetUserByIdAsync(userId);
            if (CurrentUser == null)
                return RedirectToPage("/Account/Login");

            IsDefaultPassword = await _userService.IsDefaultPasswordAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ các trường mật khẩu.";
                return RedirectToPage();
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return RedirectToPage();
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải dài ít nhất 6 ký tự.";
                return RedirectToPage();
            }

            var userId = GetCurrentUserId();
            try
            {
                var success = await _userService.ChangePasswordAsync(userId, oldPassword, newPassword);
                TempData[success ? "Success" : "Error"] = success
                    ? "Đổi mật khẩu thành công!"
                    : "Mật khẩu cũ không chính xác.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi đổi mật khẩu: {ex.Message}";
            }

            return RedirectToPage();
        }

        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            int.TryParse(userIdString, out int userId);
            return userId;
        }
    }
}
