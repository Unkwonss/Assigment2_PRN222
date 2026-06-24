using System;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) của phiên trò chuyện học tập.
    /// </summary>
    public class ChatSessionDto
    {
        /// <summary>
        /// Mã định danh duy nhất (Guid) của phiên chat.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Id của học sinh hoặc giáo viên sở hữu phiên chat này.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id môn học tương ứng của phiên trò chuyện.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// Tiêu đề của phiên trò chuyện (ví dụ: Giải đáp chương 1 PRN222).
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Tóm tắt tóm lược nội dung cuộc hội thoại do AI tự động tổng hợp.
        /// </summary>
        public string? ConversationSummary { get; set; }

        /// <summary>
        /// Thời điểm bắt đầu tạo phiên chat.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Thời điểm trao đổi tin nhắn cuối cùng trong phiên chat.
        /// </summary>
        public DateTime? LastUpdatedAt { get; set; }

        /// <summary>
        /// Danh sách lịch sử tin nhắn trong phiên chat này.
        /// </summary>
        public virtual ICollection<ChatHistoryDto> ChatHistories { get; set; } = new List<ChatHistoryDto>();

        /// <summary>
        /// Thông tin môn học của phiên chat.
        /// </summary>
        public virtual SubjectDto? Subject { get; set; }

        /// <summary>
        /// Người dùng sở hữu phiên chat.
        /// </summary>
        public virtual UserDto? User { get; set; }
    }
}
