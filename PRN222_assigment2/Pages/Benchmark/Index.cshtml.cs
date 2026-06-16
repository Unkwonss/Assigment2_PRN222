using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using PRN222_assigment2.Models;

namespace PRN222_assigment2.Pages.Benchmark
{
    [Authorize(Roles = "Teacher,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IBenchmarkService _benchmarkService;
        private readonly IDocumentService _documentService;

        public IndexModel(IBenchmarkService benchmarkService, IDocumentService documentService)
        {
            _benchmarkService = benchmarkService;
            _documentService = documentService;
        }

        public IEnumerable<ExperimentDto> Experiments { get; set; } = new List<ExperimentDto>();
        public IEnumerable<SubjectDto> Subjects { get; set; } = new List<SubjectDto>();
        public IEnumerable<AimodelDto> AIModels { get; set; } = new List<AimodelDto>();
        public IEnumerable<EmbeddingModelDto> EmbeddingModels { get; set; } = new List<EmbeddingModelDto>();
        public IEnumerable<ChunkingStrategyDto> Strategies { get; set; } = new List<ChunkingStrategyDto>();

        public async Task OnGetAsync()
        {
            Subjects = await _documentService.GetAllSubjectsAsync();
            Experiments = await _benchmarkService.GetAllExperimentsAsync();
            AIModels = await _benchmarkService.GetAllAIModelsAsync();
            EmbeddingModels = await _documentService.GetAllEmbeddingModelsAsync();
            Strategies = await _documentService.GetAllChunkingStrategiesAsync();
        }

        public async Task<IActionResult> OnPostCreateExperimentAsync(string experimentName, string experimentDescription, int aiModelId, int? embeddingModelId, int? strategyId, int? chunkSize, int? chunkOverlap)
        {
            if (string.IsNullOrEmpty(experimentName) || aiModelId <= 0)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ tên thử nghiệm và cấu hình mô hình AI.";
                return RedirectToPage();
            }

            var exp = new ExperimentDto
            {
                ExperimentName        = experimentName,
                ExperimentDescription = experimentDescription,
                AimodelId             = aiModelId,
                EmbeddingModelId      = embeddingModelId,
                StrategyId            = strategyId,
                ChunkSize             = chunkSize,
                ChunkOverlap          = chunkOverlap
            };

            try
            {
                await _benchmarkService.CreateExperimentAsync(exp);
                TempData["Success"] = "Khởi tạo thử nghiệm nghiên cứu thành công!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Tên thử nghiệm đã tồn tại hoặc cấu hình không hợp lệ.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteExperimentAsync(int experimentId)
        {
            try
            {
                await _benchmarkService.DeleteExperimentAsync(experimentId);
                TempData["Success"] = "Xóa thử nghiệm thành công!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Không thể xóa thử nghiệm này.";
            }

            return RedirectToPage();
        }

        // AJAX: GET test sets
        public async Task<IActionResult> OnGetGetTestSetsAsync(int subjectId)
        {
            var testSets = await _benchmarkService.GetTestSetsBySubjectIdAsync(subjectId);
            return new JsonResult(testSets.Select(t => new
            {
                t.QuestionId,
                t.Question,
                t.GroundTruth,
                CreatedAt = t.CreatedAt?.ToString("dd/MM/yyyy")
            }));
        }

        // AJAX: POST create test set
        public async Task<IActionResult> OnPostCreateTestSetAsync(int subjectId, string question, string groundTruth)
        {
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(groundTruth))
                return new JsonResult(new { success = false, message = "Câu hỏi và đáp án chuẩn không được để trống." });

            var ts = new TestSetDto
            {
                SubjectId   = subjectId,
                Question    = question,
                GroundTruth = groundTruth,
                CreatedAt   = DateTime.UtcNow
            };

            await _benchmarkService.CreateTestSetAsync(ts);
            return new JsonResult(new { success = true, message = "Thêm câu hỏi mẫu thành công!" });
        }

        // AJAX: POST edit test set
        public async Task<IActionResult> OnPostEditTestSetAsync(int questionId, string question, string groundTruth)
        {
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(groundTruth))
                return new JsonResult(new { success = false, message = "Thông tin không hợp lệ." });

            var existing = await _benchmarkService.GetTestSetByIdAsync(questionId);
            if (existing == null) return new JsonResult(new { success = false, message = "Câu hỏi không tồn tại." });

            existing.Question    = question;
            existing.GroundTruth = groundTruth;
            await _benchmarkService.UpdateTestSetAsync(existing);
            return new JsonResult(new { success = true, message = "Cập nhật câu hỏi thành công!" });
        }

        // AJAX: POST delete test set
        public async Task<IActionResult> OnPostDeleteTestSetAsync(int questionId)
        {
            try
            {
                await _benchmarkService.DeleteTestSetAsync(questionId);
                return new JsonResult(new { success = true, message = "Xóa câu hỏi kiểm thử thành công!" });
            }
            catch (Exception)
            {
                return new JsonResult(new { success = false, message = "Lỗi khi xóa câu hỏi." });
            }
        }

        // AJAX: POST run benchmark
        public async Task<IActionResult> OnPostRunBenchmarkAsync(int experimentId, int subjectId)
        {
            try
            {
                var results = await _benchmarkService.RunBenchmarkAsync(experimentId, subjectId);
                if (!results.Any())
                    return new JsonResult(new { success = false, message = "Môn học được chọn hiện tại chưa có bộ câu hỏi kiểm thử." });
                return new JsonResult(new { success = true, message = $"Chạy thành công bộ đánh giá RBL ({results.Count()} câu hỏi)!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi thực thi thử nghiệm: {ex.Message}" });
            }
        }

        // AJAX: GET benchmark results
        public async Task<IActionResult> OnGetGetBenchmarkResultsAsync(int experimentId)
        {
            var results = await _benchmarkService.GetResultsByExperimentIdAsync(experimentId);
            return new JsonResult(results.Select(r => new
            {
                r.ResultId,
                QuestionText    = r.Question?.Question ?? "Câu hỏi",
                GroundTruthText = r.Question?.GroundTruth ?? "Đáp án chuẩn",
                r.GeneratedResponse,
                r.LatencyMilliseconds,
                r.TokensIn,
                r.TokensOut,
                Faithfulness = r.FaithfulnessScore ?? 0.0,
                Relevance    = r.AnswerRelevanceScore ?? 0.0,
                Precision    = r.ContextPrecisionScore ?? 0.0,
                Recall       = r.ContextRecallScore ?? 0.0,
                TestedAt     = r.TestedAt?.ToString("dd/MM/yyyy HH:mm")
            }));
        }

        // AJAX: GET all benchmark results
        public async Task<IActionResult> OnGetGetAllBenchmarkResultsAsync()
        {
            var results = await _benchmarkService.GetAllResultsAsync();
            return new JsonResult(results.Select(r => new
            {
                r.ResultId,
                ExperimentName = r.Experiment?.ExperimentName ?? "Thử nghiệm",
                ModelType      = r.Experiment?.Aimodel?.ModelType ?? "Base-RAG",
                QuestionText   = r.Question?.Question ?? "Câu hỏi",
                r.LatencyMilliseconds,
                r.TokensIn,
                r.TokensOut,
                Faithfulness = r.FaithfulnessScore ?? 0.0,
                Relevance    = r.AnswerRelevanceScore ?? 0.0,
                Precision    = r.ContextPrecisionScore ?? 0.0,
                Recall       = r.ContextRecallScore ?? 0.0
            }));
        }
    }
}
