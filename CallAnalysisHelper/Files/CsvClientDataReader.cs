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

                    // Проверяем другие поля, в которых могут оказаться еще номера телефонов
                    // если есть, то добавляем к существующему списку
                    string[] otherPhoneNumberFields = { 
                        "Рабочий прямой телефон",
                        "Мобильный телефон",
                        "Факс",
                        "Домашний телефон",
                        "Другой телефон" 
                    };

                    for (int i = 0; i < otherPhoneNumberFields.Length; i++)
                    {
                        string? currentField = csv.GetField<string>(otherPhoneNumberFields[i]);

                        if (!string.IsNullOrEmpty(currentField))
                        {
                            phoneNumbers += $",{csv.GetField<string>(otherPhoneNumberFields[i])}";
                        }
                    }
                    

                    var formatedPhoneNumbers = string.Join(",", phoneNumbers.Split(',')
                                            .Select(phone => PhoneNumberParser.NormalizePhoneNumber(phone.Trim())));

                    var inn = csv.GetField<string>("ИНН");
                    var ogrn = csv.GetField<string>("ОГРН");

                    clients.Add(new Client
                    {
                        Client_Id = id,
                        Client_CompanyName = companyName,
                        Client_PhoneNumbers = formatedPhoneNumbers,
                        Client_INN = inn,
                        Client_OGRN = ogrn
                    });
                }
            }

            return clients;
        }
    }
}
