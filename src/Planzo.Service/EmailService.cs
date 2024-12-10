using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Planzo.Service.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Planzo.Data.Configurations;

namespace Planzo.Service;

public class EmailService:IEmailService
{
    private readonly UserManager<IdentityUser> _userManager;

    public EmailService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
    }
    
    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.mailosaur.net", 587, false);
        await client.AuthenticateAsync("kvr09sgs@mailosaur.net" , "our_mailosaur_password");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Planzo App", "noreply@planzo.com"));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Password Reset Request";

        message.Body = new TextPart("plain")
        {
            Text = $"Click the following link to reset your password: " +
                   $"https://myfrontend.com/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}"
        };

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}