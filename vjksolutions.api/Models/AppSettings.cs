namespace vjksolutions.api.Models;

public class AppSettings
{
    public string ApiKey { get; set; }
    public string Sender { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Server { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string Domain { get; set; }
    public string ServerKey { get; set; }

    public AppSettings()
    {
        ApiKey = string.Empty;
        Sender = string.Empty;
        Recipient = string.Empty;
        Subject = string.Empty;
        Server = string.Empty;
        User = string.Empty;
        Password = string.Empty;
        Domain = string.Empty;
        ServerKey = string.Empty;
    }
}
