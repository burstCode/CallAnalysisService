namespace CallAnalysisHelper.Models
{
    public class CallRecord
    {
        public int Id { get; set; }

        public bool IsMissed { get; set; }

        public string ClientPhoneNumber { get; set; }
        
        public string SupportAgentName { get; set; }
        
        public DateTime CallDate { get; set; }
        public DateTime CallTime { get; set; }
        public TimeSpan CallDuration { get; set; }
    }

}
