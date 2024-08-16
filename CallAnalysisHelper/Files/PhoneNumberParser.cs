using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CallAnalysisHelper.Files
{
    public static class PhoneNumberParser
    {
        // Нормализация номера телефона до вида 7XXXXXXXXXX
        public static string NormalizePhoneNumber(string phoneNumber /* здесь будет передаваться ОГРН */)
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
    }
}
