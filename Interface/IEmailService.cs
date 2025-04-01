using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OloEcomm.Interface
{
    public interface IEmailService
    {
         Task SendEmailAsync (string toEmail, string subject, string body);
    }
}