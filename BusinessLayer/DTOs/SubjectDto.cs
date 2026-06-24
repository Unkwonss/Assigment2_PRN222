using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) đại diện cho một môn học.
    /// </summary>
    public class SubjectDto
    {
        /// <summary>
        /// Mã định danh của môn học.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// Mã ký hiệu môn học (Ví dụ: PRN222, SWP391, PRN212).
        /// </summary>
        public string SubjectCode { get; set; } = null!;

        /// <summary>
        /// Tên đầy đủ của môn học (Ví dụ: C# Programming and Web Development).
        /// </summary>
        public string SubjectName { get; set; } = null!;
        
        /// <summary>
        /// Id của Giáo viên quản lý môn học này (Trưởng bộ môn).
        /// </summary>
        public int? ManagedByUserId { get; set; }

        /// <summary>
        /// Tên tài khoản của Giáo viên quản lý môn học.
        /// </summary>
        public string? ManagedByUserName { get; set; }
        
        /// <summary>
        /// Danh sách Id của các giáo viên được phân công phụ trách môn học.
        /// </summary>
        public List<int> AssignedTeacherIds { get; set; } = new List<int>();

        /// <summary>
        /// Danh sách đối tượng DTO của các giáo viên phụ trách môn học.
        /// </summary>
        public List<UserDto> AssignedTeachers { get; set; } = new List<UserDto>();

        /// <summary>
        /// Danh sách các chương học thuộc môn học này.
        /// </summary>
        public virtual ICollection<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();

        /// <summary>
        /// Danh sách các phiên trò chuyện học tập thuộc môn học này.
        /// </summary>
        public virtual ICollection<ChatSessionDto> ChatSessions { get; set; } = new List<ChatSessionDto>();

        /// <summary>
        /// Danh sách các câu hỏi kiểm thử thuộc môn học này.
        /// </summary>
        public virtual ICollection<TestSetDto> TestSets { get; set; } = new List<TestSetDto>();
    }
}
