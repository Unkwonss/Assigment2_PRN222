using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;
using PRN222_assigment2.Models;

namespace PRN222_assigment2.Pages.Benchmark
{
    [Authorize(Roles = "Teacher,Admin")]
    public class DashboardModel : PageModel
    {
        private readonly IBenchmarkService _benchmarkService;

        public DashboardModel(IBenchmarkService benchmarkService)
        {
            _benchmarkService = benchmarkService;
        }

        public List<BenchmarkResultViewModel> Results { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allResults = await _benchmarkService.GetAllResultsAsync();

            Results = allResults.Select(r => new BenchmarkResultViewModel
            {
                ModelName     = r.Experiment?.Aimodel?.ModelName ?? "Unknown Model",
                ChunkStrategy = r.Experiment?.Strategy?.StrategyName ?? "Default",
                Precision3    = r.ContextPrecisionScore ?? 0,
                Recall3       = r.ContextRecallScore ?? 0,
                MRR           = r.FaithfulnessScore ?? 0,
                AvgLatencyMs  = r.LatencyMilliseconds,
                RunAt         = r.TestedAt ?? DateTime.UtcNow
            }).ToList();
        }
    }
}
