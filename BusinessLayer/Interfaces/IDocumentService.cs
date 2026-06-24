using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Định nghĩa các nghiệp vụ liên quan đến quản lý môn học, chương học, tài liệu học tập và chỉ mục hóa văn bản (indexing).
    /// </summary>
    public interface IDocumentService
    {
        // Subjects
        
        /// <summary>
        /// Lấy danh sách tất cả môn học.
        /// </summary>
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();

        /// <summary>
        /// Lấy thông tin chi tiết môn học qua Id.
        /// </summary>
        Task<SubjectDto?> GetSubjectByIdAsync(int id);

        /// <summary>
        /// Tạo một môn học mới.
        /// </summary>
        Task<SubjectDto> CreateSubjectAsync(SubjectDto subjectDto);

        /// <summary>
        /// Cập nhật thông tin môn học.
        /// </summary>
        Task UpdateSubjectAsync(SubjectDto subjectDto);

        /// <summary>
        /// Xóa môn học ra khỏi hệ thống.
        /// </summary>
        Task DeleteSubjectAsync(int id);

        /// <summary>
        /// Kiểm tra xem người dùng (giáo viên/sinh viên) có được phân bổ cho môn học hay không.
        /// </summary>
        Task<bool> IsUserAssignedToSubjectAsync(int userId, int subjectId);

        /// <summary>
        /// Kiểm tra xem người dùng có phải Chủ nhiệm bộ môn (Subject Head) của môn học hay không.
        /// </summary>
        Task<bool> IsUserSubjectHeadAsync(int userId, int subjectId);

        /// <summary>
        /// Kiểm tra xem người dùng có phải Chủ nhiệm bộ môn quản lý chương học cụ thể hay không.
        /// </summary>
        Task<bool> IsUserSubjectHeadForChapterAsync(int userId, int chapterId);

        /// <summary>
        /// Phân công danh sách giáo viên phụ trách môn học và thiết lập chủ nhiệm bộ môn.
        /// </summary>
        Task AssignTeachersToSubjectAsync(int subjectId, List<int> teacherIds, int? headTeacherId);

        /// <summary>
        /// Lấy danh sách các giáo viên phụ trách môn học cụ thể.
        /// </summary>
        Task<IEnumerable<UserDto>> GetTeachersBySubjectIdAsync(int subjectId);

        // Chapters

        /// <summary>
        /// Lấy danh sách các chương học thuộc về một môn học cụ thể.
        /// </summary>
        Task<IEnumerable<ChapterDto>> GetChaptersBySubjectIdAsync(int subjectId);

        /// <summary>
        /// Lấy chi tiết chương học qua Id.
        /// </summary>
        Task<ChapterDto?> GetChapterByIdAsync(int id);

        /// <summary>
        /// Tạo chương học mới cho một môn học.
        /// </summary>
        Task<ChapterDto> CreateChapterAsync(ChapterDto chapterDto);

        /// <summary>
        /// Cập nhật thông tin chương học.
        /// </summary>
        Task UpdateChapterAsync(ChapterDto chapterDto);

        /// <summary>
        /// Xóa chương học và toàn bộ tài liệu trực thuộc chương đó.
        /// </summary>
        Task DeleteChapterAsync(int id);

        // Documents

        /// <summary>
        /// Lấy danh sách tài liệu thuộc về một chương học cụ thể.
        /// </summary>
        Task<IEnumerable<DocumentDto>> GetDocumentsByChapterIdAsync(int chapterId);

        /// <summary>
        /// Lấy danh sách các tài liệu đã được lập chỉ mục thành công của môn học.
        /// </summary>
        Task<IEnumerable<DocumentDto>> GetIndexedDocumentsAsync(int subjectId);

        /// <summary>
        /// Lấy thông tin tài liệu qua Id.
        /// </summary>
        Task<DocumentDto?> GetDocumentByIdAsync(int id);

        /// <summary>
        /// Tải lên một tài liệu mới cùng nội dung văn bản trích xuất được.
        /// </summary>
        Task<DocumentDto> UploadDocumentAsync(DocumentDto documentDto, string textContent);

        /// <summary>
        /// Xóa tài liệu khỏi hệ thống.
        /// </summary>
        Task DeleteDocumentAsync(int id);

        /// <summary>
        /// Lấy trạng thái quá trình tạo embedding cho tài liệu.
        /// </summary>
        Task<string> GetEmbeddingStatusAsync(int documentId);

        // Chunking Strategies & Embedding Models

        /// <summary>
        /// Lấy toàn bộ các chiến lược phân đoạn văn bản được cấu hình trong hệ thống.
        /// </summary>
        Task<IEnumerable<ChunkingStrategyDto>> GetAllChunkingStrategiesAsync();

        /// <summary>
        /// Lấy danh sách các mô hình Vector nhúng được hỗ trợ.
        /// </summary>
        Task<IEnumerable<EmbeddingModelDto>> GetAllEmbeddingModelsAsync();

        // Indexing & Chunking

        /// <summary>
        /// Thực hiện phân đoạn văn bản và tạo cơ sở dữ liệu vector chỉ mục (Indexing) cho tài liệu.
        /// </summary>
        Task<DocumentIndexDto> IndexDocumentAsync(int documentId, int modelId, int strategyId, int chunkSize, int chunkOverlap);

        /// <summary>
        /// Lấy danh sách các chỉ mục đã cấu hình cho tài liệu.
        /// </summary>
        Task<IEnumerable<DocumentIndexDto>> GetIndexesByDocumentIdAsync(int documentId);

        /// <summary>
        /// Lấy danh sách các phân đoạn văn bản (chunks) thuộc về một chỉ mục.
        /// </summary>
        Task<IEnumerable<DocumentChunkDto>> GetChunksByIndexIdAsync(int indexId);
    }
}
