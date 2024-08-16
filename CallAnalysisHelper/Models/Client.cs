using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAnalysisHelper.Models
{
    public class Client
    {
        [Key]
        public int Client_Id { get; set; }

        public string Client_CompanyName { get; set; }

        public string Client_PhoneNumbers { get; set; }

        [NotMapped]
        public List<string> PhoneNumberList
        {
            get => Client_PhoneNumbers?.Split(',').ToList() ?? new List<string>();
            set => Client_PhoneNumbers = string.Join(",", value);
        }
    }
}
