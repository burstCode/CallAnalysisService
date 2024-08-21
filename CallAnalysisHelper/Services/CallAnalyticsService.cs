using CallAnalysisHelper.Database;
using CallAnalysisHelper.Models.Statistics;
using CallAnalysisHelper.Files;
using System.Text.RegularExpressions;

namespace CallAnalysisHelper.Services
{
    public class CallAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelCallDataReader _excelReader;
        private readonly CsvClientDataReader _csvReader;


        public CallAnalyticsService(ApplicationDbContext context, ExcelCallDataReader excelReader, CsvClientDataReader csvReader)
        {
            _context = context;
            _excelReader = excelReader;
            _csvReader = csvReader;

            // Пока для примера
            ImportData("", "");
        }


        public void ImportData(string callDataFilePath, string clientDataFilePath)
        {
            callDataFilePath = "C:\\Users\\mikex\\Downloads\\Istoria_vneshnikh_zvonkov_12-08-2024.xlsx";
            clientDataFilePath = "C:\\Users\\mikex\\Downloads\\amocrm_export_companies_2024-08-12_1.csv";

            var calls = _excelReader.ReadCallDataFromExcel(callDataFilePath);
            var clients = _csvReader.ReadClientDataFromCsv(clientDataFilePath);

            _context.ImportCallRecords(calls);
            _context.ImportClients(clients);
        }



        // Получение клиентов с самыми длинными звонками
        public List<ClientCallDuration> GetClientsWithLongestCalls()
        {
            var clients = _context.Clients
                .AsEnumerable()
                .Select(client => new
                {
                    Client = client,
                    PhoneNumbers = client.PhoneNumberList
                })
                .ToList();

            return _context.CallRecords
                .AsEnumerable()
                .GroupBy(record =>
                {
                    var client = clients.FirstOrDefault(c => c.PhoneNumbers.Contains(record.Call_ClientPhoneNumber));
                    return client?.Client.Client_CompanyName ?? record.Call_ClientPhoneNumber;
                })
                .Select(g => new ClientCallDuration
                {
                    CompanyName = g.Key,
                    TotalDuration = g.Sum(r => r.Call_Duration.TotalMinutes)
                })
                .OrderByDescending(c => c.TotalDuration)
                .ToList();
        }



        // Получение клиентов с наибольшим количеством звонков
        public List<ClientCallCount> GetClientsWithMostCalls()
        {
            var clients = _context.Clients
                .AsEnumerable()
                .Select(client => new
                {
                    Client = client,
                    PhoneNumbers = client.PhoneNumberList
                })
                .ToList();

            return _context.CallRecords
                .AsEnumerable()
                .GroupBy(record =>
                {
                    var client = clients.FirstOrDefault(c => c.PhoneNumbers.Contains(record.Call_ClientPhoneNumber));
                    return client?.Client.Client_CompanyName ?? record.Call_ClientPhoneNumber;
                })
                .Select(g => new ClientCallCount
                {
                    CompanyName = g.Key,
                    CallCount = g.Count()
                })
                .OrderByDescending(c => c.CallCount)
                .ToList();
        }



        // Распределение звонкам по типам
        public Dictionary<string, int> GetCallTypeDistribution()
        {
            return _context.CallRecords
                .AsEnumerable()
                .GroupBy(c => c.Call_Type)
                .Select(g => new
                {
                    CallType = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(k => k.CallType, v => v.Count);
        }
        



        // Загрузка сотрудников тех поддержки
        public List<SupportAgentLoad> GetSupportAgentLoad()
        {
            return _context.CallRecords
                .AsEnumerable()
                .GroupBy(c => c.Call_SupportAgentName)
                .Select(g => new SupportAgentLoad
                {
                    SupportAgentName = g.Key,
                    TotalDuration = g.Sum(c => c.Call_Duration.TotalMinutes)
                })
                .OrderByDescending(c => c.TotalDuration)
                .ToList();
        }



        // Клиенты, от которых звонков не поступало
        public List<ClientWithoutCalls> GetClientsWithoutCalls()
        {
            var clientsWithCalls = _context.CallRecords
                .AsEnumerable()
                .Select(record => record.Call_ClientPhoneNumber)
                .Distinct()
                .ToList();

            return _context.Clients
                .AsEnumerable()
                .Where(client => !client.PhoneNumberList.Any(phone => clientsWithCalls.Contains(phone)))
                .Select(client => new ClientWithoutCalls
                {
                    CompanyName = client.Client_CompanyName,
                    PhoneNumber = client.Client_PhoneNumbers
                })
                .ToList();
        }



        // Статистика по времени ожидания :D
        public WaitingTimeStatistics GetWaitingTimeStatistics()
        {
            List<TimeSpan> waitingTimes = _context.CallRecords
                .AsEnumerable()
                .Select(record => record.Call_WaitingTime)
                .ToList();

            return new WaitingTimeStatistics
            {
                AverageWaitingTime = CalculateAverageWaitingTime(waitingTimes),
                MaxWaitingTime = waitingTimes.Max().TotalSeconds,
                MinWaitingTime = waitingTimes.Min().TotalSeconds
            };
        }

        public double CalculateAverageWaitingTime(List<TimeSpan> waitingTimes)
        {
            double totalSeconds = waitingTimes.Sum(time => time.TotalSeconds);

            return totalSeconds / waitingTimes.Count;
        }
    }
}
