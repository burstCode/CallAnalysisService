namespace CallAnalysisHelper.Models
{
    public class CallRecord
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string SupportAgentName { get; set; }
        public DateTime CallDate { get; set; }
        public TimeSpan CallDuration { get; set; }
        public bool IsMissed { get; set; }
    }

}
