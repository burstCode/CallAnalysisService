using System.ComponentModel.DataAnnotations;

namespace CallAnalysisHelper.Models
{
    public class CallRecord
    {
        [Key]
        public int Call_Id { get; set; }


        public string Call_Type { get; set; }

        public string Call_ClientPhoneNumber { get; set; }
        
        public string Call_SupportAgentName { get; set; }

        public string Call_ThroughPhoneNumber { get; set; }
        
        public DateTime Call_Date { get; set; }

        public DateTime Call_Time { get; set; }

        public TimeSpan Call_WaitingTime { get; set; }

        public TimeSpan Call_Duration { get; set; }
    }

}
