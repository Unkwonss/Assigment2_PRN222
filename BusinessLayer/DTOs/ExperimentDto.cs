using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) cấu hình thực nghiệm đánh giá hiệu năng RAG.
    /// </summary>
    public class ExperimentDto
    {
        /// <summary>
        /// Mã định danh thực nghiệm.
        /// </summary>
        public int ExperimentId { get; set; }

        /// <summary>
        /// Tên của thực nghiệm (Ví dụ: Gemini + Recursive + Size 1000).
        /// </summary>
        public string ExperimentName { get; set; } = null!;

        /// <summary>
        /// Mô tả chi tiết mục tiêu hoặc giả thuyết thực nghiệm.
        /// </summary>
        public string? ExperimentDescription { get; set; }

        /// <summary>
        /// Id của mô hình ngôn ngữ lớn (LLM) được sử dụng để trả lời câu hỏi.
        /// </summary>
        public int AimodelId { get; set; }

        /// <summary>
        /// Id của mô hình vector hóa (Embedding) được sử dụng để mã hóa dữ liệu.
        /// </summary>
        public int? EmbeddingModelId { get; set; }

        /// <summary>
        /// Id của chiến lược phân đoạn văn bản (Chunking).
        /// </summary>
        public int? StrategyId { get; set; }

        /// <summary>
        /// Độ dài tối đa của một phân đoạn văn bản (tính bằng ký tự hoặc từ).
        /// </summary>
        public int? ChunkSize { get; set; }

        /// <summary>
        /// Độ dài trùng lặp giữa hai phân đoạn văn bản liên tiếp.
        /// </summary>
        public int? ChunkOverlap { get; set; }

        /// <summary>
        /// Thông tin DTO mô hình sinh ngôn ngữ được cấu hình.
        /// </summary>
        public virtual AimodelDto? Aimodel { get; set; }

        /// <summary>
        /// Danh sách kết quả benchmark tự động của thực nghiệm này.
        /// </summary>
        public virtual ICollection<BenchmarkResultDto> BenchmarkResults { get; set; } = new List<BenchmarkResultDto>();

        /// <summary>
        /// Thông tin DTO mô hình Embedding được cấu hình.
        /// </summary>
        public virtual EmbeddingModelDto? EmbeddingModel { get; set; }

        /// <summary>
        /// Thông tin DTO chiến lược Chunking được cấu hình.
        /// </summary>
        public virtual ChunkingStrategyDto? Strategy { get; set; }
    }
}
