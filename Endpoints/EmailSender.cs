using System;

namespace WebApp.Endpoints;

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebApp.Models;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _emailConfig;

    public EmailSender(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }

public async Task SendEmailAsync(string email, string subject, string message)
{
    if (string.IsNullOrEmpty(email))
    {
        throw new ArgumentException("Email address cannot be null or empty.", nameof(email));
    }

    var mailMessage = new MailMessage
    {
        From = new MailAddress("support@passwordmanager.com", "Password Manager"),
        Subject = subject,
        Body = message,
        IsBodyHtml = true
    };
    mailMessage.To.Add(new MailAddress(email));

    using (var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io", 2525))
    {
        smtpClient.Credentials = new NetworkCredential("b1ef2320fb1cd1", "85302ebefddb29");
        smtpClient.EnableSsl = true;

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            // Log the specific exception message
            Console.WriteLine($"SMTP Exception: {ex.Message}");
            throw; // Rethrow or handle accordingly
        }
    }
}


}
