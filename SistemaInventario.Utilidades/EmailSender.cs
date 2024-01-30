using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilidades
{
    public class EmailSender : IEmailSender
    {

        public string SendGridSecret  { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            SendGridSecret = configuration.GetValue<string>("Sendgrid:SecretKey");
        }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var cliente = new SendGridClient(SendGridSecret);
            var from = new EmailAddress("ivancarrieri@yahoo.com.ar");
            var to = new EmailAddress(email);
            var mensaje = MailHelper.CreateSingleEmail(from, to, subject,"", htmlMessage);

            return cliente.SendEmailAsync(mensaje);
            
        }   
    }
}
