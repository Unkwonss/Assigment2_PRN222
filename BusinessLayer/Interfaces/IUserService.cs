using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Định nghĩa các nghiệp vụ liên quan đến quản lý người dùng và xác thực tài khoản.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Xác thực người dùng bằng email và mật khẩu.
        /// </summary>
        Task<UserDto?> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Đăng ký tài khoản người dùng mới.
        /// </summary>
        Task<UserDto?> RegisterAsync(string username, string password, string fullName, string email, string role = "Student");

        /// <summary>
        /// Lấy thông tin chi tiết người dùng qua Id.
        /// </summary>
        Task<UserDto?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Tìm kiếm người dùng bằng địa chỉ email.
        /// </summary>
        Task<UserDto?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Lấy danh sách tất cả người dùng trong hệ thống.
        /// </summary>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Tạo một người dùng mới từ đối tượng DTO.
        /// </summary>
        Task<UserDto> CreateUserAsync(UserDto userDto);

        /// <summary>
        /// Cập nhật thông tin của người dùng hiện tại.
        /// </summary>
        Task UpdateUserAsync(UserDto userDto);

        /// <summary>
        /// Xóa người dùng ra khỏi hệ thống qua Id.
        /// </summary>
        Task DeleteUserAsync(int userId);

        /// <summary>
        /// Tạo nhanh tài khoản cho sinh viên mới.
        /// </summary>
        Task<UserDto?> CreateStudentAccountAsync(string fullName, string email);

        /// <summary>
        /// Nhập danh sách sinh viên từ file CSV.
        /// </summary>
        Task<int> ImportStudentsFromCsvAsync(System.IO.Stream fileStream);

        /// <summary>
        /// Thay đổi mật khẩu người dùng.
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

        /// <summary>
        /// Kiểm tra xem người dùng có đang dùng mật khẩu mặc định hay không.
        /// </summary>
        Task<bool> IsDefaultPasswordAsync(int userId);
    }
}
