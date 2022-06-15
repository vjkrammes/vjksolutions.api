using FluentEmail.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Text;

using vjksolutions.api.Models;

namespace vjksolutions.api.Infrastructure;

public static class ExtensionMethods
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v2/Email/Send", SendEmail);
    }

    private static void FormatText(this string text, StringBuilder sb, int linelength = 80)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        if (text.Length <= linelength)
        {
            sb.AppendLine(text);
            return;
        }
        var words = text.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var spaceleft = linelength;
        var index = 0;
        var thisline = new StringBuilder();
        while (index < words.Length)
        {
            var word = words[index];
            if (spaceleft - word.Length < 0)
            {
                sb.Append(thisline);
                sb.AppendLine();
                thisline = new(word);
                spaceleft = linelength - word.Length;
            }
            else
            {
                thisline.Append(word);
                spaceleft -= word.Length;
            }
            thisline.Append(' ');
            spaceleft--;
            index++;
        }
        sb.AppendLine(thisline.ToString());
    }

    private static async Task<IResult> SendEmail([FromBody] EmailModel model, IOptions<AppSettings> settings)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.SenderEmail) || string.IsNullOrWhiteSpace(model.SenderName) ||
            string.IsNullOrWhiteSpace(model.Subject) || string.IsNullOrWhiteSpace(model.Text))
        {
            return Results.BadRequest(new string[] { "Invalid model passed. It is either null or is missing required fields " });
        }
        if (model.ApiKey != settings.Value.ApiKey)
        {
            return Results.BadRequest(new string[] { "You are not authorized to perform that action " });
        }
        StringBuilder sb = new($"New Email from {model.SenderName} ({model.SenderEmail})");
        sb.AppendLine();
        sb.AppendLine($"Subject: ${model.Subject}");
        sb.AppendLine();
        sb.AppendLine("Text --------------------------------------------------------------------------");
        model.Text.FormatText(sb);
        sb.AppendLine("-------------------------------------------------------------------------------");
        sb.AppendLine();
        if (!string.IsNullOrWhiteSpace(model.SenderPhone))
        {
            sb.Append($"Phone: {model.SenderPhone}");
            if (model.VoiceCall)
            {
                sb.Append(", Voice OK");
            }
            if (model.SMS)
            {
                sb.Append(", SMS OK");
            }
            if (!string.IsNullOrWhiteSpace(model.Timezone))
            {
                sb.AppendLine();
                sb.Append($"Time Zone: {model.Timezone}");
            }
            sb.AppendLine();
        }
        if (!string.IsNullOrWhiteSpace(model.Interest))
        {
            sb.AppendLine($"Interest: {model.Interest}");
        }
        sb.AppendLine();
        var text = sb.ToString();
        var email = Email
            .From(settings.Value.Sender, "VJK Solutions")
            .To(settings.Value.Recipient)
            .Subject(settings.Value.Subject)
            .Body(text);
        var result = await email.SendAsync();
        if (result.Successful)
        {
            return Results.Ok(result.MessageId ?? "Message sent successfully");
        }
        return Results.BadRequest(result.ErrorMessages.ToArray());
    }
}
