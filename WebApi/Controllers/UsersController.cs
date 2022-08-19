using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ILogger<ProfileController> _logger;

    public UsersController(ILogger<ProfileController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [RequiredScope("Users.View")]
    public async Task<string[]> GetUsers([FromServices] IDownstreamWebApi apiHelper)
    {
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        try
        {
            var response = await apiHelper.CallWebApiForAppAsync(
                "GraphTest",
                o =>
                {
                    o.Scopes = string.Join(" ", scopes);
                    o.RelativePath = "v1.0/users";
                    o.HttpMethod = HttpMethod.Get;
                });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var items = new List<string>();

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("value", out var valueCollection))
            {
                foreach (var value in valueCollection.EnumerateArray())
                {
                    var name = value.GetProperty("displayName").GetString();
                    var mail = value.GetProperty("mail").GetString();
                    var principalName = value.GetProperty("userPrincipalName").GetString();
                    var id = value.GetProperty("id").GetString();
                    items.Add(name ?? mail ?? principalName ?? id ?? "* Unknown");
                }
            }
            return items.ToArray();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error calling the downstream API");
            throw;
        }
    }
}