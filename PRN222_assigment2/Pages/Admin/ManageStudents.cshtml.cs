using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;

namespace PRN222_assigment2.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageStudentsModel : PageModel
    {
        private readonly IUserService _userService;

        public ManageStudentsModel(IUserService userService)
        {
            _userService = userService;
        }

        public IEnumerable<BusinessLayer.DTOs.UserDto> Students { get; set; } = new List<BusinessLayer.DTOs.UserDto>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var allUsers = await _userService.GetAllUsersAsync();
            var students = allUsers.Where(u => u.Role == "Student");

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                students = students.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.Username != null && u.Username.ToLower().Contains(term)));
            }

            Students = students;
        }

        public async Task<IActionResult> OnPostAddStudentAsync(string fullName, string email)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ họ tên và email.";
                return RedirectToPage();
            }

            if (!email.Contains("@"))
            {
                TempData["Error"] = "Email không hợp lệ.";
                return RedirectToPage();
            }

            try
            {
                var existing = await _userService.GetUserByEmailAsync(email);
                if (existing != null)
                {
                    TempData["Error"] = $"Email '{email}' đã tồn tại trong hệ thống với vai trò '{existing.Role}'.";
                    return RedirectToPage();
                }

                var student = await _userService.CreateStudentAccountAsync(fullName, email);
                TempData[student != null ? "Success" : "Error"] = student != null
                    ? $"Đã tạo tài khoản cho sinh viên '{fullName}'. Username: '{student.Username}'. Thông tin tài khoản đã được gửi qua email."
                    : "Tạo tài khoản thất bại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostImportCsvAsync(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một file CSV hợp lệ.";
                return RedirectToPage();
            }

            string ext = Path.GetExtension(csvFile.FileName).ToLower();
            if (ext != ".csv" && ext != ".txt")
            {
                TempData["Error"] = "Chỉ chấp nhận file định dạng .csv hoặc .txt chứa dữ liệu CSV.";
                return RedirectToPage();
            }

            try
            {
                using var stream = csvFile.OpenReadStream();
                int successCount = await _userService.ImportStudentsFromCsvAsync(stream);
                TempData[successCount > 0 ? "Success" : "Error"] = successCount > 0
                    ? $"Import thành công {successCount} sinh viên và gửi email tài khoản mặc định!"
                    : "Không import được sinh viên nào. Vui lòng kiểm tra lại cấu trúc file CSV.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi import file CSV: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
