using CallAnalysisHelper.Models.Statistics;
using CallAnalysisHelper.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace CallAnalysisWeb.Pages.Stats
{
    public class StatsModel : PageModel
    {
        private readonly CallAnalyticsService _analyticsService;

        public List<ClientCallDuration> ClientsWithLongestCalls { get; set; }
        public List<ClientCallCount> ClientsWithMostCalls { get; set; }
        public List<SupportAgentLoad> SupportAgentLoad { get; set; }
        public Dictionary<string, int> CallTypeDistribution { get; set; }

        // Пока что пришлось явно указать, так как видит такой же класс в страницах разор
        public List<ClientWithoutCalls> ClientsWithoutCalls { get; set; }

        public StatsModel(CallAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public void OnGet()
        {
            ClientsWithLongestCalls = _analyticsService.GetClientsWithLongestCalls();
            ClientsWithMostCalls = _analyticsService.GetClientsWithMostCalls();
            SupportAgentLoad = _analyticsService.GetSupportAgentLoad();
            ClientsWithoutCalls = _analyticsService.GetClientsWithoutCalls();
            CallTypeDistribution = _analyticsService.GetCallTypeDistribution();
        }
    }
}
