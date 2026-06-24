using System;

namespace BusinessLayer.DTOs
{
    /// <summary>
    /// Đối tượng vận chuyển dữ liệu (DTO) kết quả đánh giá (Benchmark) hiệu năng.
    /// </summary>
    public class BenchmarkResultDto
    {
        /// <summary>
        /// Mã định danh kết quả đánh giá.
        /// </summary>
        public int ResultId { get; set; }

        /// <summary>
        /// Id của câu hỏi kiểm thử được đánh giá.
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Id của thực nghiệm được kiểm tra.
        /// </summary>
        public int ExperimentId { get; set; }

        /// <summary>
        /// Câu trả lời được sinh ra bởi mô hình AI trong thực nghiệm.
        /// </summary>
        public string GeneratedResponse { get; set; } = null!;

        /// <summary>
        /// Độ trễ xử lý (phản hồi) tính bằng mili-giây.
        /// </summary>
        public int LatencyMilliseconds { get; set; }

        /// <summary>
        /// Số lượng token đầu vào (prompt tokens) được sử dụng.
        /// </summary>
        public int? TokensIn { get; set; }

        /// <summary>
        /// Số lượng token đầu ra (completion tokens) được sinh ra.
        /// </summary>
        public int? TokensOut { get; set; }

        /// <summary>
        /// Thông tin lỗi (nếu có) phát sinh trong quá trình gọi API AI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Điểm số đo lường mức độ trung thực của câu trả lời so với ngữ cảnh (Faithfulness).
        /// </summary>
        public double? FaithfulnessScore { get; set; }

        /// <summary>
        /// Điểm số đo lường mức độ liên quan trực tiếp của câu trả lời với câu hỏi (Answer Relevance).
        /// </summary>
        public double? AnswerRelevanceScore { get; set; }

        /// <summary>
        /// Điểm số đo lường độ chính xác của ngữ cảnh được truy xuất (Context Precision).
        /// </summary>
        public double? ContextPrecisionScore { get; set; }

        /// <summary>
        /// Điểm số đo lường tỷ lệ thông tin được truy xuất so với thông tin chuẩn cần thiết (Context Recall).
        /// </summary>
        public double? ContextRecallScore { get; set; }

        /// <summary>
        /// Thời điểm thực hiện chạy thử nghiệm đánh giá.
        /// </summary>
        public DateTime? TestedAt { get; set; }

        /// <summary>
        /// Đối tượng DTO thực nghiệm tương ứng.
        /// </summary>
        public virtual ExperimentDto? Experiment { get; set; }

        /// <summary>
        /// Đối tượng DTO câu hỏi kiểm thử trong bộ đề mẫu tương ứng.
        /// </summary>
        public virtual TestSetDto? Question { get; set; }
    }
}
