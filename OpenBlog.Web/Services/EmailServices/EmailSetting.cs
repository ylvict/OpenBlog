namespace OpenBlog.Web.Services.EmailServices
{
    public class EmailSetting
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string IllegalChars { get; set; }
    }
}

    