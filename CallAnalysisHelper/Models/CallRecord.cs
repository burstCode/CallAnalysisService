using System.ComponentModel.DataAnnotations;

namespace CallAnalysisHelper.Models
{
    public class CallRecord
    {
        [Key]
        public int Call_Id { get; set; }


        public bool Call_IsMissed { get; set; }

        public string Call_ClientPhoneNumber { get; set; }
        
        public string Call_SupportAgentName { get; set; }
        
        public DateTime Call_CallDate { get; set; }
        public DateTime Call_CallTime { get; set; }
        public TimeSpan Call_CallDuration { get; set; }
    }

}
