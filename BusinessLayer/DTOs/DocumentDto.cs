using System;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) đại diện cho tài liệu học tập trong hệ thống.
    /// </summary>
    public class DocumentDto
    {
        /// <summary>
        /// Mã định danh của tài liệu.
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Mã chương học trực thuộc.
        /// </summary>
        public int ChapterId { get; set; }

        /// <summary>
        /// Tiêu đề hiển thị của tài liệu.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Tên tệp gốc khi tải lên.
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Đường dẫn lưu trữ tệp trên hệ thống tập tin máy chủ.
        /// </summary>
        public string FilePath { get; set; } = null!;

        /// <summary>
        /// Định dạng mở rộng của tệp (ví dụ: PDF, TXT).
        /// </summary>
        public string FileType { get; set; } = null!;

        /// <summary>
        /// Kích thước của tệp tính bằng byte.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Tổng số trang của tài liệu (nếu là PDF).
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Trạng thái xử lý của tài liệu (ví dụ: Pending, Processed).
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// Id của giáo viên tải tài liệu này lên.
        /// </summary>
        public int UploadedBy { get; set; }

        /// <summary>
        /// Thời điểm tải tài liệu lên hệ thống.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Đối tượng DTO chương học trực thuộc tài liệu.
        /// </summary>
        public virtual ChapterDto? Chapter { get; set; }

        /// <summary>
        /// Danh sách các phiên bản chỉ mục (indexing) đã được tạo cho tài liệu này.
        /// </summary>
        public virtual ICollection<DocumentIndexDto> DocumentIndices { get; set; } = new List<DocumentIndexDto>();

        /// <summary>
        /// Đối tượng DTO người dùng đã tải tài liệu này lên.
        /// </summary>
        public virtual UserDto? UploadedByNavigation { get; set; }
    }
}
