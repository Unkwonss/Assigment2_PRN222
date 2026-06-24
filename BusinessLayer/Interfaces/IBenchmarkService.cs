using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Định nghĩa các nghiệp vụ liên quan đến thử nghiệm (Experiments), bộ đề mẫu (Test Sets) và đánh giá hiệu năng (Benchmarking).
    /// </summary>
    public interface IBenchmarkService
    {
        // Experiments
        
        /// <summary>
        /// Lấy danh sách tất cả các thực nghiệm (cấu hình RAG) trong hệ thống.
        /// </summary>
        Task<IEnumerable<ExperimentDto>> GetAllExperimentsAsync();

        /// <summary>
        /// Lấy chi tiết thông tin thực nghiệm qua Id.
        /// </summary>
        Task<ExperimentDto?> GetExperimentByIdAsync(int id);

        /// <summary>
        /// Tạo mới một thực nghiệm (cấu hình RAG).
        /// </summary>
        Task<ExperimentDto> CreateExperimentAsync(ExperimentDto experimentDto);

        /// <summary>
        /// Cập nhật cấu hình thực nghiệm.
        /// </summary>
        Task UpdateExperimentAsync(ExperimentDto experimentDto);

        /// <summary>
        /// Xóa một thực nghiệm ra khỏi hệ thống.
        /// </summary>
        Task DeleteExperimentAsync(int id);

        // Test Sets (Questions & Ground Truth)

        /// <summary>
        /// Lấy danh sách bộ đề mẫu (câu hỏi và câu trả lời chuẩn) của môn học cụ thể.
        /// </summary>
        Task<IEnumerable<TestSetDto>> GetTestSetsBySubjectIdAsync(int subjectId);

        /// <summary>
        /// Lấy chi tiết bộ câu hỏi kiểm thử qua Id.
        /// </summary>
        Task<TestSetDto?> GetTestSetByIdAsync(int id);

        /// <summary>
        /// Tạo mới câu hỏi kiểm thử trong bộ đề mẫu.
        /// </summary>
        Task<TestSetDto> CreateTestSetAsync(TestSetDto testSetDto);

        /// <summary>
        /// Cập nhật nội dung câu hỏi kiểm thử.
        /// </summary>
        Task UpdateTestSetAsync(TestSetDto testSetDto);

        /// <summary>
        /// Xóa câu hỏi kiểm thử khỏi bộ đề mẫu.
        /// </summary>
        Task DeleteTestSetAsync(int id);

        // AI Models

        /// <summary>
        /// Lấy danh sách các mô hình ngôn ngữ lớn (LLM) hỗ trợ sinh văn bản.
        /// </summary>
        Task<IEnumerable<AimodelDto>> GetAllAIModelsAsync();

        // Running Benchmarks

        /// <summary>
        /// Khởi chạy đánh giá hiệu năng cấu hình thực nghiệm đối với bộ đề mẫu của môn học cụ thể.
        /// </summary>
        Task<IEnumerable<BenchmarkResultDto>> RunBenchmarkAsync(int experimentId, int subjectId);

        /// <summary>
        /// Lấy kết quả benchmark của một thực nghiệm cụ thể.
        /// </summary>
        Task<IEnumerable<BenchmarkResultDto>> GetResultsByExperimentIdAsync(int experimentId);

        /// <summary>
        /// Lấy toàn bộ lịch sử kết quả benchmark trong hệ thống.
        /// </summary>
        Task<IEnumerable<BenchmarkResultDto>> GetAllResultsAsync();
    }
}
