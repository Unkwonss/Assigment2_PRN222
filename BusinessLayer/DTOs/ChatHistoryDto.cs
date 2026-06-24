using System;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) của lịch sử tin nhắn trong phiên chat.
    /// </summary>
    public class ChatHistoryDto
    {
        /// <summary>
        /// Mã định danh lịch sử tin nhắn.
        /// </summary>
        public int HistoryId { get; set; }

        /// <summary>
        /// Mã định danh phiên chat chứa tin nhắn này.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Nội dung tin nhắn của người dùng gửi đi.
        /// </summary>
        public string UserMessage { get; set; } = null!;

        /// <summary>
        /// Nội dung truy vấn đã được tinh chỉnh (Standalone Query) phục vụ việc tìm kiếm ngữ cảnh.
        /// </summary>
        public string? StandaloneQuery { get; set; }

        /// <summary>
        /// Câu trả lời phản hồi từ hệ thống RAG/AI.
        /// </summary>
        public string BotResponse { get; set; } = null!;

        /// <summary>
        /// Thời điểm tin nhắn được trao đổi.
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Danh sách các nguồn trích dẫn tài liệu (Citations) được dùng để làm căn cứ cho câu trả lời.
        /// </summary>
        public virtual ICollection<ChatCitationDto> ChatCitations { get; set; } = new List<ChatCitationDto>();

        /// <summary>
        /// Thông tin phiên trò chuyện chứa tin nhắn này.
        /// </summary>
        public virtual ChatSessionDto? Session { get; set; }
    }
}
