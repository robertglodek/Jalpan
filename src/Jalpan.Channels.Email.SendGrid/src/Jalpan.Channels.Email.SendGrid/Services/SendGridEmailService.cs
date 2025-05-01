using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Jalpan.Channels.Email.SendGrid.Services;

internal sealed class SendGridEmailService(ISendGridClient sendGridClient, IOptions<SendGridOptions> options) : IEmailService
{
    public void SendEmailAsync(string emailTo, string nameTo, string emailFrom, string nameFrom, string subject, string plainTextContent, string htmlContent)
    {
        var to = new EmailAddress(emailTo, nameTo);
        var from = new EmailAddress(emailFrom, nameFrom);

        var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = sendGridClient.SendEmailAsync(message);



    }

    public void SendEmailAsync(string emailTo, string nameTo, string emailFrom, string nameFrom, string templateId, object dynamicTemplateData)
    {
        var to = new EmailAddress(emailTo, nameTo);
        var from = new EmailAddress(emailFrom, nameFrom);

        var message = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);

        var response = sendGridClient.SendEmailAsync(message);



    }
}
