using CallAnalysisHelper.Database;
using CallAnalysisHelper.Models.Statistics;

namespace CallAnalysisHelper.Services
{
    public class CallAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public CallAnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ClientCallDuration> GetClientsWithLongestCalls()
        {
            return _context.CallRecords
                .AsEnumerable()  // Преобразуем запрос в Enumerable, чтобы выполнить дальнейшие операции на стороне клиента
                .GroupBy(cr => cr.ClientName)
                .Select(g => new ClientCallDuration
                {
                    ClientName = g.Key,
                    TotalDuration = g.Sum(cr => cr.CallDuration.TotalMinutes)
                })
                .OrderByDescending(c => c.TotalDuration)
                .ToList();
        }



        public List<ClientCallCount> GetClientsWithMostCalls()
        {
            return _context.CallRecords
                .GroupBy(c => c.ClientName)
                .Select(g => new ClientCallCount
                {
                    ClientName = g.Key,
                    CallCount = g.Count()
                })
                .OrderByDescending(c => c.CallCount)
                .ToList();
        }

        public CallStatistics GetCallStatistics()
        {
            var totalCalls = _context.CallRecords.Count();
            var missedCalls = _context.CallRecords.Count(c => c.IsMissed);
            var receivedCalls = totalCalls - missedCalls;

            return new CallStatistics
            {
                TotalCalls = totalCalls,
                MissedCalls = missedCalls,
                ReceivedCalls = receivedCalls
            };
        }

        public int GetAcceptedCallsCount()
        {
            return _context.CallRecords.Count(cr => !cr.IsMissed);
        }

        public int GetMissedCallsCount()
        {
            return _context.CallRecords.Count(cr => cr.IsMissed);
        }

        public List<SupportAgentLoad> GetSupportAgentLoad()
        {
            return _context.CallRecords
                .AsEnumerable()
                .GroupBy(c => c.SupportAgentName)
                .Select(g => new SupportAgentLoad
                {
                    SupportAgentName = g.Key,
                    TotalDuration = g.Sum(c => c.CallDuration.TotalMinutes)
                })
                .OrderByDescending(c => c.TotalDuration)
                .ToList();
        }

        public List<ClientWithoutCalls> GetClientsWithoutCalls()
        {
            var clientsWithCalls = _context.CallRecords
                .Select(cr => cr.ClientName)
                .Distinct()
                .ToList();

            return _context.Clients
                .Where(c => !clientsWithCalls.Contains(c.CompanyName))
                .Select(c => new ClientWithoutCalls
                {
                    CompanyName = c.CompanyName,
                    PhoneNumber = c.PhoneNumber
                })
                .ToList();
        }
    }
}