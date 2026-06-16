using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using PRN222_assigment2.Models;

namespace PRN222_assigment2.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly IUserService _userService;

        public UsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public UserSearchViewModel ViewModel { get; set; } = new();

        [BindProperty(SupportsGet = true, Name = "sortBy")]
        public string SortBy { get; set; } = "fullName";

        [BindProperty(SupportsGet = true, Name = "sortOrder")]
        public string SortOrder { get; set; } = "asc";

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        public async Task OnGetAsync()
        {
            var allUsers = await _userService.GetAllUsersAsync();
            IEnumerable<UserDto> filteredUsers = allUsers;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                filteredUsers = filteredUsers.Where(u =>
                    u.FullName.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term) ||
                    u.Username.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(RoleFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Role == RoleFilter);
            }

            filteredUsers = SortBy.ToLower() switch
            {
                "email"    => SortOrder == "desc" ? filteredUsers.OrderByDescending(u => u.Email)    : filteredUsers.OrderBy(u => u.Email),
                "role"     => SortOrder == "desc" ? filteredUsers.OrderByDescending(u => u.Role)     : filteredUsers.OrderBy(u => u.Role),
                "username" => SortOrder == "desc" ? filteredUsers.OrderByDescending(u => u.Username) : filteredUsers.OrderBy(u => u.Username),
                _          => SortOrder == "desc" ? filteredUsers.OrderByDescending(u => u.FullName) : filteredUsers.OrderBy(u => u.FullName),
            };

            ViewModel = new UserSearchViewModel
            {
                SearchTerm    = SearchTerm,
                RoleFilter    = RoleFilter,
                Users         = filteredUsers,
                TotalCount    = allUsers.Count(),
                FilteredCount = filteredUsers.Count()
            };
        }

        public async Task<IActionResult> OnPostCreateUserAsync(CreateUserViewModel model)
        {
            if (model.Role != "Student" && string.IsNullOrWhiteSpace(model.Password))
            {
                TempData["Error"] = "Mật khẩu là bắt buộc đối với Giáo viên hoặc Admin.";
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return RedirectToPage();
            }

            string password = model.Password ?? string.Empty;
            if (model.Role == "Student" && string.IsNullOrWhiteSpace(password))
                password = "FptStudent@123";

            var user = new UserDto
            {
                Username     = model.Username.Trim(),
                PasswordHash = password,
                FullName     = model.FullName.Trim(),
                Email        = model.Email.Trim().ToLower(),
                Role         = model.Role
            };

            var result = await _userService.CreateUserAsync(user);
            TempData[result == null ? "Error" : "Success"] = result == null
                ? "Tên đăng nhập hoặc Email đã được sử dụng trong hệ thống."
                : $"Tạo tài khoản '{model.FullName}' thành công!";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditUserAsync(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return RedirectToPage();
            }

            var user = new UserDto
            {
                UserId       = model.UserId,
                Username     = model.Username.Trim(),
                PasswordHash = model.Password ?? string.Empty,
                FullName     = model.FullName.Trim(),
                Email        = model.Email.Trim().ToLower(),
                Role         = model.Role
            };

            await _userService.UpdateUserAsync(user);
            TempData["Success"] = $"Cập nhật tài khoản '{model.FullName}' thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int userId)
        {
            if (userId <= 0)
            {
                TempData["Error"] = "ID tài khoản không hợp lệ.";
                return RedirectToPage();
            }

            await _userService.DeleteUserAsync(userId);
            TempData["Success"] = "Xóa tài khoản thành công!";
            return RedirectToPage();
        }
    }
}
