using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Interfaces;
using System.Security.Claims;

namespace PRN222_assigment2.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IDocumentService _documentService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, IDocumentService documentService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _documentService = documentService;
            _logger = logger;
        }

        /// <summary>
        /// Khi client kết nối, tự động join group theo userId để nhận thông báo real-time
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            if (userId > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                _logger.LogInformation("[SignalR] User {UserId} connected. ConnectionId={ConnectionId}", userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetCurrentUserId();
            if (userId > 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                _logger.LogInformation("[SignalR] User {UserId} disconnected. ConnectionId={ConnectionId}", userId, Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Client join vào group của session để nhận tin nhắn real-time
        /// </summary>
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session_{sessionId}");
            _logger.LogInformation("[SignalR] ConnectionId={ConnectionId} joined session {SessionId}", Context.ConnectionId, sessionId);
        }

        /// <summary>
        /// Client rời group session
        /// </summary>
        public async Task LeaveSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session_{sessionId}");
        }

        /// <summary>
        /// Gửi tin nhắn chat qua SignalR - xử lý và phản hồi real-time
        /// </summary>
        public async Task SendMessage(Guid sessionId, string message, int modelId, int strategyId, int chunkSize, int chunkOverlap)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var userId = GetCurrentUserId();
            var timestamp = DateTime.Now.ToString("HH:mm");

            // 1. Gửi tin nhắn user đến tất cả client trong session (real-time)
            await Clients.Group($"session_{sessionId}").SendAsync("ReceiveUserMessage", new
            {
                userId,
                message,
                timestamp
            });

            // 2. Gửi trạng thái "đang xử lý" (typing indicator)
            await Clients.Group($"session_{sessionId}").SendAsync("BotTyping", new
            {
                sessionId = sessionId.ToString(),
                isTyping = true
            });

            try
            {
                // 3. Gọi service xử lý chat
                var result = await _chatService.SendMessageWithScoresAsync(sessionId, message, modelId, strategyId, chunkSize, chunkOverlap);
                var historyRecord = result.History;

                var responseData = new
                {
                    success = true,
                    historyId = historyRecord.HistoryId,
                    userMessage = historyRecord.UserMessage,
                    botResponse = historyRecord.BotResponse,
                    timestamp = historyRecord.Timestamp?.ToString("HH:mm"),
                    citations = result.Citations.Select(x => new
                    {
                        x.Citation.CitationId,
                        x.Citation.ChunkId,
                        x.Citation.PageNumber,
                        Snippet = x.Citation.Snippet ?? "",
                        DocumentTitle = x.Citation.Chunk?.Index?.Document?.Title ?? "Tài liệu học tập",
                        SimilarityScore = x.Score,
                        ScorePercent = (int)(x.Score * 100)
                    })
                };

                // 4. Gửi phản hồi bot đến tất cả client trong session (real-time)
                await Clients.Group($"session_{sessionId}").SendAsync("ReceiveBotResponse", responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SignalR] Chat error for session {SessionId}", sessionId);

                // Gửi thông báo lỗi real-time
                await Clients.Group($"session_{sessionId}").SendAsync("ReceiveBotResponse", new
                {
                    success = false,
                    message = ex.Message
                });
            }
            finally
            {
                // 5. Tắt typing indicator
                await Clients.Group($"session_{sessionId}").SendAsync("BotTyping", new
                {
                    sessionId = sessionId.ToString(),
                    isTyping = false
                });
            }
        }

        /// <summary>
        /// Thông báo real-time khi có session mới được tạo
        /// </summary>
        public async Task NotifyNewSession(int subjectId, Guid sessionId, string title)
        {
            var userId = GetCurrentUserId();
            await Clients.Group($"user_{userId}").SendAsync("SessionCreated", new
            {
                sessionId,
                title,
                subjectId,
                lastUpdatedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            });
        }

        /// <summary>
        /// Thông báo real-time khi session bị xóa
        /// </summary>
        public async Task NotifySessionDeleted(Guid sessionId)
        {
            var userId = GetCurrentUserId();
            await Clients.Group($"user_{userId}").SendAsync("SessionDeleted", new
            {
                sessionId
            });
        }

        /// <summary>
        /// Thông báo real-time khi session được đổi tên
        /// </summary>
        public async Task NotifySessionRenamed(Guid sessionId, string newTitle)
        {
            var userId = GetCurrentUserId();
            await Clients.Group($"user_{userId}").SendAsync("SessionRenamed", new
            {
                sessionId,
                newTitle
            });
        }

        private int GetCurrentUserId()
        {
            var userIdString = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            int.TryParse(userIdString, out int userId);
            return userId;
        }
    }
}
