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

            // Чтение данных о звонках
            var callRecords = _excelReader.ReadCallDataFromExcel(callDataFilePath);

            // Чтение данных о клиентах
            var clients = _csvReader.ReadClientDataFromCsv(clientDataFilePath);

            // Сохранение данных в базу
            _context.CallRecords.AddRange(callRecords);
            _context.Clients.AddRange(clients);
            _context.SaveChanges();
        }




        // Форматирование (нормализация) номера телефона до вида 7XXXXXXXXXX
        static string FormatPhoneNumber(string phoneNumber /* здесь будет передаваться */)
        {
            string cleanedNumber = Regex.Replace(phoneNumber, @"\D", "");

            if (cleanedNumber.Length == 6)
            {
                // В этом случае либо помещать в столбец "подозрительный номер",
                // либо заморочиться и автоматически дописывать по первым двум цифрам ОГРН вроде
            }

            // Рассматриваем случай, когда одной цифры не хватает, например (4742) 72-67-81
            if (cleanedNumber.Length == 10 && !cleanedNumber.StartsWith("7"))
            {
                cleanedNumber = "7" + cleanedNumber;
            }

            // Если номер начинается с восьмерки, меняем на семерку
            if (cleanedNumber.StartsWith("8"))
            {
                cleanedNumber = "7" + cleanedNumber.Substring(1);
            }

            return cleanedNumber;
        }





        // Получение клиентов с самыми длинными звонками
        public List<ClientCallDuration> GetClientsWithLongestCalls()
        {
            var clients = _context.Clients
                .AsEnumerable()
                .Select(client => new
                {
                    Client = client,
                    FormatedPhoneNumbers = client.PhoneNumberList
                        .Select(phone => FormatPhoneNumber(phone)).ToList()
                })
                .ToList();

            return _context.CallRecords
                .AsEnumerable()
                .Select(record => new
                {
                    Record = record,
                    FormatedPhoneNumber = FormatPhoneNumber(record.ClientPhoneNumber)
                })
                .GroupBy(r => clients.FirstOrDefault(c => c.FormatedPhoneNumbers.Contains(r.FormatedPhoneNumber))?.Client.CompanyName)
                .Select(g => new ClientCallDuration
                {
                    ClientName = g.Key,
                    TotalDuration = g.Sum(r => r.Record.CallDuration.TotalMinutes)
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
                    FormatedPhoneNumbers = client.PhoneNumberList
                        .Select(phone => FormatPhoneNumber(phone)).ToList()
                })
                .ToList();

            return _context.CallRecords
                .AsEnumerable()
                .Select(record => new
                {
                    Record = record,
                    FormatedPhoneNumber = FormatPhoneNumber(record.ClientPhoneNumber)
                })
                .GroupBy(r => clients.FirstOrDefault(c => c.FormatedPhoneNumbers.Contains(r.FormatedPhoneNumber))?.Client.CompanyName)
                .Select(g => new ClientCallCount
                {
                    ClientName = g.Key,
                    CallCount = g.Count()
                })
                .OrderByDescending(c => c.CallCount)
                .ToList();
        }




        // Статистика принятых и пропущенных звонков
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



        // Загрузка сотрудников тех поддержки
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



        // Клиенты, от которых звонков не поступало
        public List<ClientWithoutCalls> GetClientsWithoutCalls()
        {
            var clientsWithCalls = _context.CallRecords
                .AsEnumerable()
                .Select(record => FormatPhoneNumber(record.ClientPhoneNumber))
                .Distinct()
                .ToList();

            return _context.Clients
                .AsEnumerable()
                .Where(client => !client.PhoneNumberList.Any(phone => clientsWithCalls.Contains(FormatPhoneNumber(phone))))
                .Select(client => new ClientWithoutCalls
                {
                    CompanyName = client.CompanyName,
                    PhoneNumber = client.PhoneNumbers
                })
                .ToList();
        }


    }
}
