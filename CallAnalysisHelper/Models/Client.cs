using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAnalysisHelper.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumbers { get; set; }

        [NotMapped]
        public List<string> PhoneNumberList
        {
            get => PhoneNumbers?.Split(',').ToList() ?? new List<string>();
            set => PhoneNumbers = string.Join(",", value);
        }
    }
}
