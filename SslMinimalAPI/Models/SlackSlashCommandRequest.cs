namespace SslMinimalAPI.Models;

public class SlackSlashCommandRequest
{
    public string Token { get; set; }

    public string TeamId { get; set; }

    public string TeamDomain { get; set; }

    public string EnterpriseId { get; set; }

    public string EnterpriseName { get; set; }

    public string ChannelId { get; set; }

    public string ChannelName { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Command { get; set; }

    public string Text { get; set; }

    public string ResponseUrl { get; set; }

    public string TriggerId { get; set; }

}
