namespace vjksolutions.api.Models;

public class EmailModel
{
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public string SenderPhone { get; set; }
    public bool VoiceCall { get; set; }
    public bool SMS { get; set; }
    public string Timezone { get; set; }
    public string Interest { get; set; }
    public string ApiKey { get; set; }

    public EmailModel()
    {
        SenderName = string.Empty;
        SenderEmail = string.Empty;
        Subject = string.Empty;
        Text = string.Empty;
        SenderPhone = string.Empty;
        VoiceCall = false;
        SMS = false;
        Timezone = string.Empty;
        Interest = string.Empty;
        ApiKey = string.Empty;
    }
}
