using CallAnalysisHelper.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace CallAnalysisHelper.Files
{
    public class CsvClientDataReader
    {
        public List<Client> ReadClientDataFromCsv(string filePath)
        {
            var clients = new List<Client>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var id = csv.GetField<int>("ID");
                    var companyName = csv.GetField<string>("Название компании");
                    var phoneNumbers = csv.GetField<string>("Рабочий телефон");

                    clients.Add(new Client
                    {
                        Id = id,
                        CompanyName = companyName,
                        PhoneNumbers = phoneNumbers
                    });
                }
            }

            return clients;
        }
    }
}
