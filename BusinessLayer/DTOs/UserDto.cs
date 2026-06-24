namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) của người dùng trong hệ thống.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Mã định danh duy nhất của người dùng.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Tên tài khoản dùng để đăng nhập.
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// Chuỗi băm mật khẩu bảo mật (SHA-256).
        /// </summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Họ và tên đầy đủ của người dùng.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Địa chỉ thư điện tử duy nhất trong hệ thống.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Vai trò phân quyền trong hệ thống (ví dụ: Admin, Teacher, Student).
        /// </summary>
        public string Role { get; set; } = null!;
    }
}
