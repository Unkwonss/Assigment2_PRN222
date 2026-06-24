using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;

namespace PRN222_assigment2.Pages.Chat
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IChatService _chatService;
        private readonly IDocumentService _documentService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IChatService chatService, IDocumentService documentService, ILogger<IndexModel> logger)
        {
            _chatService = chatService;
            _documentService = documentService;
            _logger = logger;
        }

        public IEnumerable<SubjectDto> Subjects { get; set; } = new List<SubjectDto>();
        public IEnumerable<ChunkingStrategyDto> Strategies { get; set; } = new List<ChunkingStrategyDto>();
        public IEnumerable<EmbeddingModelDto> EmbeddingModels { get; set; } = new List<EmbeddingModelDto>();

        public async Task OnGetAsync()
        {
            Subjects = await _documentService.GetAllSubjectsAsync();
            Strategies = await _documentService.GetAllChunkingStrategiesAsync();
            EmbeddingModels = await _documentService.GetAllEmbeddingModelsAsync();
        }

        // AJAX: GET sessions
        public async Task<IActionResult> OnGetGetSessionsAsync(int subjectId)
        {
            var userId = GetCurrentUserId();
            var sessions = await _chatService.GetSessionsAsync(userId, subjectId);
            return new JsonResult(sessions.Select(s => new
            {
                s.SessionId,
                s.Title,
                LastUpdatedAt = s.LastUpdatedAt?.ToString("dd/MM/yyyy HH:mm")
            }));
        }

        // AJAX: POST create session
        public async Task<IActionResult> OnPostCreateSessionAsync(int subjectId, string title)
        {
            var userId = GetCurrentUserId();
            var name = string.IsNullOrEmpty(title) ? "Cuộc trò chuyện mới" : title;
            var session = await _chatService.CreateSessionAsync(userId, subjectId, name);
            return new JsonResult(new { success = true, sessionId = session.SessionId, title = session.Title });
        }

        // AJAX: POST rename session
        public async Task<IActionResult> OnPostRenameSessionAsync(Guid sessionId, string newTitle)
        {
            if (string.IsNullOrEmpty(newTitle))
                return new JsonResult(new { success = false, message = "Tiêu đề không hợp lệ." });
            await _chatService.RenameSessionAsync(sessionId, newTitle);
            return new JsonResult(new { success = true });
        }

        // AJAX: POST delete session
        public async Task<IActionResult> OnPostDeleteSessionAsync(Guid sessionId)
        {
            await _chatService.DeleteSessionAsync(sessionId);
            return new JsonResult(new { success = true });
        }

        // AJAX: GET session history
        public async Task<IActionResult> OnGetGetSessionHistoryAsync(Guid sessionId)
        {
            var history = await _chatService.GetChatHistoryAsync(sessionId);
            return new JsonResult(history.Select(h => new
            {
                h.HistoryId,
                h.UserMessage,
                h.BotResponse,
                Timestamp = h.Timestamp?.ToString("HH:mm"),
                Citations = h.ChatCitations.Select(c => new
                {
                    c.CitationId,
                    c.ChunkId,
                    c.PageNumber,
                    Snippet = c.Snippet ?? "",
                    DocumentTitle = c.Chunk?.Index?.Document?.Title ?? "Tài liệu học tập"
                })
            }));
        }

        // GET: Export session history to Markdown/Text file
        public async Task<IActionResult> OnGetExportSessionAsync(Guid sessionId)
        {
            var history = await _chatService.GetChatHistoryAsync(sessionId);
            var session = await _chatService.GetSessionByIdAsync(sessionId);
            if (session == null)
            {
                return NotFound("Không tìm thấy cuộc trò chuyện.");
            }

            var title = session.Title ?? "Cuoc_tro_chuyen";
            var sb = new StringBuilder();
            sb.AppendLine($"# Lịch sử hội thoại: {title}");
            sb.AppendLine($"- **Môn học**: {session.Subject?.SubjectCode} - {session.Subject?.SubjectName}");
            sb.AppendLine($"- **Ngày xuất**: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            foreach (var h in history)
            {
                // Format timestamp (+7 UTC for local time)
                var localTime = h.Timestamp?.AddHours(7).ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                sb.AppendLine($"### 👤 Người dùng ({localTime}):");
                sb.AppendLine(h.UserMessage);
                sb.AppendLine();
                sb.AppendLine($"### 🤖 Trợ lý RAG ({localTime}):");
                sb.AppendLine(h.BotResponse);
                sb.AppendLine();

                if (h.ChatCitations != null && h.ChatCitations.Any())
                {
                    sb.AppendLine("**📚 Nguồn trích dẫn:**");
                    int idx = 1;
                    foreach (var c in h.ChatCitations)
                    {
                        var docTitle = c.Chunk?.Index?.Document?.Title ?? "Tài liệu";
                        sb.AppendLine($"- [{idx++}] {docTitle} (Trang {c.PageNumber}):");
                        sb.AppendLine($"  > *\"{c.Snippet?.Replace("\n", " ").Replace("\r", "")}\"*");
                    }
                    sb.AppendLine();
                }
                sb.AppendLine("---");
                sb.AppendLine();
            }

            var cleanTitle = string.Concat(title.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
            var fileName = $"{cleanTitle}_History.md";
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/markdown", fileName);
        }

        // AJAX: POST send message
        public async Task<IActionResult> OnPostSendMessageAsync(Guid sessionId, string message, int modelId, int strategyId, int chunkSize, int chunkOverlap)
        {
            if (string.IsNullOrWhiteSpace(message))
                return new JsonResult(new { success = false, message = "Vui lòng nhập tin nhắn." });

            try
            {
                var result = await _chatService.SendMessageWithScoresAsync(sessionId, message, modelId, strategyId, chunkSize, chunkOverlap);
                var historyRecord = result.History;
                return new JsonResult(new {
                    success = true,
                    historyId = historyRecord.HistoryId,
                    userMessage = historyRecord.UserMessage,
                    botResponse = historyRecord.BotResponse,
                    timestamp = historyRecord.Timestamp?.ToString("HH:mm"),
                    citations = result.Citations.Select(x => new {
                        x.Citation.CitationId,
                        x.Citation.ChunkId,
                        x.Citation.PageNumber,
                        Snippet = x.Citation.Snippet ?? "",
                        DocumentTitle = x.Citation.Chunk?.Index?.Document?.Title ?? "Tài liệu học tập",
                        SimilarityScore = x.Score,
                        ScorePercent = (int)(x.Score * 100)
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chat error");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // AJAX: GET documents by subject (Student sidebar)
        public async Task<IActionResult> OnGetGetDocumentsBySubjectAsync(int subjectId)
        {
            if (subjectId <= 0) return new JsonResult(new List<object>());

            var chapters = await _documentService.GetChaptersBySubjectIdAsync(subjectId);
            var allDocs = new List<object>();

            foreach (var chapter in chapters)
            {
                var docs = await _documentService.GetDocumentsByChapterIdAsync(chapter.ChapterId);
                allDocs.AddRange(docs.Select(d => new
                {
                    d.DocumentId,
                    d.Title,
                    d.FileName,
                    d.FileType,
                    d.Status,
                    d.FileSize,
                    ChapterName = chapter.ChapterName,
                    IsIndexed = d.Status == "Indexed"
                }));
            }

            return new JsonResult(allDocs.OrderByDescending(d => ((dynamic)d).IsIndexed));
        }

        // AJAX: POST upload student document
        public async Task<IActionResult> OnPostUploadStudentDocumentAsync(int subjectId, string title, IFormFile file)
        {
            _logger.LogInformation("UploadStudentDocument started. SubjectId={SubjectId}, Title={Title}, FileName={FileName}, FileSize={FileSize}",
                subjectId, title, file?.FileName, file?.Length);

            if (file == null || file.Length == 0)
                return new JsonResult(new { success = false, step = 1, message = "Vui lòng chọn tệp để tải lên." });

            if (string.IsNullOrWhiteSpace(title))
                return new JsonResult(new { success = false, step = 1, message = "Vui lòng nhập tiêu đề tài liệu." });

            if (subjectId <= 0)
                return new JsonResult(new { success = false, step = 1, message = "Vui lòng chọn môn học." });

            string ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".txt" && ext != ".pdf" && ext != ".docx" && ext != ".pptx")
                return new JsonResult(new { success = false, step = 1, message = "Chỉ chấp nhận định dạng: .pdf, .docx, .pptx, .txt" });

            if (file.Length > 15 * 1024 * 1024)
                return new JsonResult(new { success = false, step = 1, message = "Kích thước tệp không được vượt quá 15MB." });

            int userId = GetCurrentUserId();
            if (userId < 0)
            {
                return new JsonResult(new { success = false, step = 1, message = "Không xác định được tài khoản đăng nhập. Vui lòng đăng nhập lại." });
            }

            ChapterDto chapter;
            try
            {
                var chapters = await _documentService.GetChaptersBySubjectIdAsync(subjectId);
                chapter = chapters.FirstOrDefault();
                if (chapter == null)
                    return new JsonResult(new { success = false, step = 2, message = "Môn học này chưa có chương nào. Vui lòng liên hệ giảng viên để tạo cấu trúc bài học." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadStudentDocument failed at step 2. SubjectId={SubjectId}. Details: {Details}",
                    subjectId, BuildExceptionDetails(ex));
                return new JsonResult(new { success = false, step = 2, message = "Lỗi khi lấy chương học. Vui lòng thử lại." });
            }

            string textContent;
            try
            {
                textContent = await ExtractTextFromFileAsync(file, ext);
                if (string.IsNullOrWhiteSpace(textContent))
                    return new JsonResult(new { success = false, step = 3, message = "Không thể trích xuất nội dung từ tệp. Tệp có thể bị rỗng hoặc được mã hóa." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadStudentDocument failed at step 3. FileName={FileName}. Details: {Details}",
                    file.FileName, BuildExceptionDetails(ex));
                return new JsonResult(new { success = false, step = 3, message = $"Lỗi trích xuất văn bản: {ex.Message}" });
            }

            var doc = new DocumentDto
            {
                ChapterId  = chapter.ChapterId,
                Title      = title.Trim(),
                FileName   = file.FileName,
                FilePath   = file.FileName,
                FileType   = ext.TrimStart('.').ToUpper(),
                FileSize   = file.Length,
                UploadedBy = userId
            };

            try
            {
                doc = await _documentService.UploadDocumentAsync(doc, textContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadStudentDocument failed at step 4. SubjectId={SubjectId}, Title={Title}. Details: {Details}",
                    subjectId, title, BuildExceptionDetails(ex));
                return new JsonResult(new { success = false, step = 4, message = $"Lỗi lưu tài liệu: {ex.Message}" });
            }

            try
            {
                var models     = await _documentService.GetAllEmbeddingModelsAsync();
                var strategies = await _documentService.GetAllChunkingStrategiesAsync();
                var model      = models.FirstOrDefault();
                var strategy   = strategies.FirstOrDefault();

                if (model != null && strategy != null)
                {
                    await _documentService.IndexDocumentAsync(
                        doc.DocumentId,
                        model.ModelId,
                        strategy.StrategyId,
                        chunkSize: 500,
                        chunkOverlap: 100
                    );
                }
                else
                {
                    return new JsonResult(new { success = false, step = 6, message = "Hệ thống chưa cấu hình Embedding Model hoặc Chunking Strategy. Vui lòng liên hệ Admin." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadStudentDocument failed at step 6. DocumentId={DocumentId}. Details: {Details}",
                    doc.DocumentId, BuildExceptionDetails(ex));
                return new JsonResult(new { success = false, step = 6, message = $"Lỗi trong quá trình lập chỉ mục RAG: {ex.Message}" });
            }

            return new JsonResult(new
            {
                success    = true,
                documentId = doc.DocumentId,
                fileName   = file.FileName,
                title      = doc.Title,
                fileType   = doc.FileType,
                message    = $"Hoàn tất! Tài liệu \"{doc.Title}\" đã sẵn sàng để truy vấn."
            });
        }

        // --- Helpers ---

        private async Task<string> ExtractTextFromFileAsync(IFormFile file, string ext)
        {
            if (ext == ".txt")
            {
                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }
            else if (ext == ".pdf")
            {
                using var document = UglyToad.PdfPig.PdfDocument.Open(file.OpenReadStream());
                var sb = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    var words = page.GetWords();
                    if (words != null && words.Any())
                    {
                        sb.AppendLine(string.Join(" ", words.Select(w => w.Text)));
                    }
                    else
                    {
                        sb.AppendLine(page.Text);
                    }
                }
                return sb.ToString();
            }
            else if (ext == ".docx")
            {
                using var archive = new System.IO.Compression.ZipArchive(file.OpenReadStream());
                var entry = archive.GetEntry("word/document.xml");
                if (entry == null) return string.Empty;

                using var entryStream = entry.Open();
                var xmlDoc = System.Xml.Linq.XDocument.Load(entryStream);
                System.Xml.Linq.XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                var paragraphs = xmlDoc.Descendants(w + "p");
                var sb = new StringBuilder();
                foreach (var p in paragraphs)
                    sb.AppendLine(string.Concat(p.Descendants(w + "t").Select(t => t.Value)));
                return sb.ToString();
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Tài liệu trình chiếu: {file.FileName}");
                try
                {
                    using var archive = new System.IO.Compression.ZipArchive(file.OpenReadStream());
                    var slideEntries = archive.Entries
                        .Where(e => e.FullName.StartsWith("ppt/slides/slide") && e.FullName.EndsWith(".xml"))
                        .OrderBy(e => e.FullName)
                        .ToList();

                    int slideNum = 1;
                    foreach (var slideEntry in slideEntries)
                    {
                        using var slideStream = slideEntry.Open();
                        var slideDoc = System.Xml.Linq.XDocument.Load(slideStream);
                        System.Xml.Linq.XNamespace a = "http://schemas.openxmlformats.org/drawingml/2006/main";
                        var texts = slideDoc.Descendants(a + "t").Select(t => t.Value).Where(t => !string.IsNullOrWhiteSpace(t));
                        sb.AppendLine($"\n--- Slide {slideNum++} ---");
                        sb.AppendLine(string.Join(" ", texts));
                    }
                }
                catch
                {
                    sb.AppendLine("(Không thể trích xuất nội dung slide — định dạng PPTX không chuẩn)");
                }
                return sb.ToString();
            }
        }

        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            int.TryParse(userIdString, out int userId);
            return userId;
        }

        private static string BuildExceptionDetails(Exception ex)
        {
            var messages = new List<string>();
            Exception current = ex;
            while (current != null)
            {
                messages.Add($"{current.GetType().Name}: {current.Message}");
                current = current.InnerException;
            }
            return string.Join(" | INNER => ", messages);
        }
    }
}
