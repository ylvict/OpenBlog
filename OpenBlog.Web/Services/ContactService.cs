using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using OpenBlog.Web.Models;
using OpenBlog.Web.Services.EmailServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBlog.Web.Services
{
    public class ContactService
    {
        private readonly EmailService _emailService;
        private readonly EmailReceivers _emailReceiversOptions;

        public ContactService(EmailService emailService, IOptions<EmailReceivers> emailReceiversOptions)
        {
            _emailService = emailService;
            _emailReceiversOptions = emailReceiversOptions.Value;
        }

        public async Task ProcessContactMessage(ContactSubmitViewModel model)
        {
            var subject = $"OpenBlog New Connect";
            var content = $"Name: {model.Name} <br/> Email:{model.Email}  <br/> Message:{model.Message}";
            await _emailService.SendEmailAsync(_emailReceiversOptions.Contacts, subject, content);
        }

        public class ContactSubmitViewModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Message { get; set; }
        }
    }
}
