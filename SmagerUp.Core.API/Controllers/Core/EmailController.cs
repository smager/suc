using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using SmagerUp.Core.API.Services;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Controllers.Core;

[ApiController]
[Route("[controller]")]
public class EmailController : SucController
{
    private readonly EmailSettingsRepository _emailSettings;

    public EmailController(EmailSettingsRepository emailSettings)
    {
        _emailSettings = emailSettings;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] EmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.To))
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = "Recipient cannot be empty."
            });
        }

        try
        {
            await SendEmailAsync(
                request.To,
                request.Subject,
                request.Body,
                request.Cc,
                request.Bcc);

            return Ok(new
            {
                isSuccess = true,
                message = "Message sent."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = ex.Message
            });
        }
    }

    private async Task SendEmailAsync(string to,string subject,string body,string? cc = null,string? bcc = null)
    {
        var info = await _emailSettings.GetInfoAsync();

        using var smtp = new SmtpClient
        {
            Host = info.Host,
            Port = info.Port,
            EnableSsl = info.IsSSL,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                info.Address,
                info.Password)
        };

        using var msg = new MailMessage
        {
            From = new MailAddress(
                info.Address,
                info.DisplayName),

            Subject = subject,
            IsBodyHtml = true
        };

        msg.To.Add(to);

        if (!string.IsNullOrWhiteSpace(cc))
            msg.CC.Add(cc);

        if (!string.IsNullOrWhiteSpace(bcc))
            msg.Bcc.Add(bcc);

        var alternateView = ContentToAlternateView(body);
        msg.AlternateViews.Add(alternateView);

        await smtp.SendMailAsync(msg);
    }

    private static AlternateView ContentToAlternateView(string content)
    {
        int imgCount = 0;

        List<LinkedResource> resources = new();

        foreach (Match match in Regex.Matches(content, "<img(?<value>.*?)>"))
        {
            imgCount++;

            var imgContent = match.Groups["value"].Value;

            string type = Regex.Match(
                imgContent,
                ":(?<type>.*?);base64,")
                .Groups["type"]
                .Value;

            string base64 = Regex.Match(
                imgContent,
                "base64,(?<base64>.*?)\"")
                .Groups["base64"]
                .Value;

            if (string.IsNullOrEmpty(type) ||
                string.IsNullOrEmpty(base64))
            {
                continue;
            }

            string replacement =
                $" src=\"cid:{imgCount}\"";

            content =
                content.Replace(imgContent, replacement);

            var resource = new LinkedResource(
                Base64ToImageStream(base64),
                new ContentType(type))
            {
                ContentId = imgCount.ToString()
            };

            resources.Add(resource);
        }

        var view =
            AlternateView.CreateAlternateViewFromString(
                content,
                null,
                MediaTypeNames.Text.Html);

        foreach (var item in resources)
        {
            view.LinkedResources.Add(item);
        }

        return view;
    }

    private static Stream Base64ToImageStream(string base64String)
    {
        byte[] bytes = Convert.FromBase64String(base64String);

        return new MemoryStream(bytes);
    }
}