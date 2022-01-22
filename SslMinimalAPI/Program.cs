using SslMinimalAPI.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var sites = builder.Configuration.GetSection("Sites").Get<List<string>>();

var app = builder.Build();
app.UseHttpsRedirection();

//Map a root endpoint because why not.
app.MapGet("/", () => { return "Hello!"; });

/// <summary>
/// Returns our SSL Expiry Data as JSON
/// </summary>
app.MapGet("/ssl", async () =>
{
    var sites = await GetSslData();  
    return JsonSerializer.Serialize(sites);
});

/// <summary>
/// An endpoint for slack to post our slack command too, and for us to return our message
/// </summary>
app.MapPost("/slack", async () =>
{
    var sslSiteList = await GetSslData();
    StringBuilder sb = new StringBuilder();

    if(sslSiteList == null || sslSiteList.Count == 0)
    {
        return "No sites found";
    }

    foreach(var sslSite in sslSiteList)
    {
        sb.Append("• " + sslSite+"\n");
    }

    return sb.ToString();
}).Accepts<SlackSlashCommandRequest>("application/x-www-form-urlencoded");

/// <summary>
/// Returns SSL Expiry Data Info from the list of sites defined in our Sites[] array in appsettings
/// </summary>
async Task<List<string>> GetSslData()
{
    DateTime expiry = DateTime.UtcNow;
    var httpClientHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) =>
        {
            expiry = cert.NotAfter;
            return true;
        }
    };
    using HttpClient httpClient = new HttpClient(httpClientHandler);
    List<string> expiryValues = new List<string>();
    foreach (var site in sites)
    {
        await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, site));
        expiryValues.Add(site + " - " + expiry);
    }
    return expiryValues;
}

app.Run();