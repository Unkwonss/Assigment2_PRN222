using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Định nghĩa các nghiệp vụ liên quan đến quản lý các phiên chat và lịch sử hội thoại.
    /// </summary>
    public interface IChatService
    {
        // Sessions
        
        /// <summary>
        /// Tạo một phiên trò chuyện (chat session) mới cho người dùng thuộc môn học cụ thể.
        /// </summary>
        Task<ChatSessionDto> CreateSessionAsync(int userId, int subjectId, string title = "Cuộc trò chuyện mới");

        /// <summary>
        /// Lấy danh sách các phiên trò chuyện của người dùng thuộc môn học cụ thể.
        /// </summary>
        Task<IEnumerable<ChatSessionDto>> GetSessionsAsync(int userId, int subjectId);

        /// <summary>
        /// Lấy thông tin chi tiết một phiên trò chuyện qua SessionId.
        /// </summary>
        Task<ChatSessionDto?> GetSessionByIdAsync(Guid sessionId);

        /// <summary>
        /// Đổi tên tiêu đề của phiên trò chuyện.
        /// </summary>
        Task RenameSessionAsync(Guid sessionId, string newTitle);

        /// <summary>
        /// Xóa hoàn toàn một phiên trò chuyện cùng toàn bộ lịch sử tin nhắn của nó.
        /// </summary>
        Task DeleteSessionAsync(Guid sessionId);

        // History and Citations

        /// <summary>
        /// Lấy toàn bộ lịch sử tin nhắn của một phiên trò chuyện.
        /// </summary>
        Task<IEnumerable<ChatHistoryDto>> GetChatHistoryAsync(Guid sessionId);

        /// <summary>
        /// Gửi câu hỏi của người dùng và nhận về câu trả lời từ chatbot RAG.
        /// </summary>
        Task<ChatHistoryDto> SendMessageAsync(Guid sessionId, string userMessage, int embeddingModelId, int strategyId, int chunkSize, int chunkOverlap);

        /// <summary>
        /// Gửi câu hỏi của người dùng và nhận câu trả lời cùng thông tin trích dẫn nguồn kèm điểm số độ tương đồng.
        /// </summary>
        Task<(ChatHistoryDto History, List<(ChatCitationDto Citation, float Score)> Citations)> SendMessageWithScoresAsync(Guid sessionId, string userMessage, int embeddingModelId, int strategyId, int chunkSize, int chunkOverlap);
    }
}
