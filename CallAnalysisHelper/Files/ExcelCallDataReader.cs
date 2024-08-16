using CallAnalysisHelper.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CallAnalysisHelper.Files
{
    public class ExcelCallDataReader
    {
        public List<CallRecord> ReadCallDataFromExcel(string filePath)
        {
            var callRecords = new List<CallRecord>();

            // Настраиваем лицензию для библиотеки
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Выбираем лист с отчетом

                // Пропускаем ненужные строки и идем до последней строки листа
                for (int row = 11; row <= worksheet.Dimension.Rows; row++)
                {
                    var isMissed = worksheet.Cells[row, 1].Text == "пропущенный" ? true : false;

                    var clientPhoneNumber = worksheet.Cells[row, 2].Text;

                    var supportAgentName = worksheet.Cells[row, 3].Text;

                    var throughPhoneNumber = worksheet.Cells[row, 5].Text;

                    var callDate = DateTime.Parse(worksheet.Cells[row, 8].Text);

                    var callTime = DateTime.Parse(worksheet.Cells[row, 9].Text);

                    var callWaitingTime = TimeSpan.Parse(worksheet.Cells[row, 10].Text);

                    var callDuration = TimeSpan.Parse(worksheet.Cells[row, 11].Text);

                    // Добавляем запись
                    callRecords.Add(new CallRecord
                    {
                        Call_IsMissed = isMissed,
                        Call_ClientPhoneNumber = clientPhoneNumber,
                        Call_SupportAgentName = supportAgentName,
                        Call_ThroughPhoneNumber = throughPhoneNumber,
                        Call_Date = callDate,
                        Call_Time = callTime,
                        Call_WaitingTime = callWaitingTime,
                        Call_Duration = callDuration,
                    });
                }
            }

            return callRecords;
        }
    }
}
