using System.Collections.Generic;
using MimeKit;

namespace OpenBlog.Web.Services.EmailServices
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<MailboxAddress>();
            FromAddresses = new List<MailboxAddress>();
        }

        public List<MailboxAddress> ToAddresses { get; set; }
        public List<MailboxAddress> FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}