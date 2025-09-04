namespace RMS.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string SupportEmail { get; set; }
        public string SupportPassword { get; set; }
        public string SendTo { get; set; }
    }
}
