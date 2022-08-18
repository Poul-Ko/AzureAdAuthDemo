using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(ILogger<ProfileController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [RequiredScope("Profile.View")]
    public async Task<ProfileData> GetProfile([FromServices] IDownstreamWebApi apiHelper)
    {
        var result = new ProfileData
        {
            Id = User.FindFirstValue(ClaimConstants.NameIdentifierId) ?? User.GetObjectId() ?? "Unknown",
            UserName = User.Identity?.Name ?? User.FindFirstValue(ClaimConstants.Name),
            PreferredName = User.FindFirstValue(ClaimConstants.PreferredUserName)
        };

        await FillNames(result, apiHelper);
        
        await FillTaskLists(result, apiHelper);

        return result;
    }

    private async Task FillNames(ProfileData result, IDownstreamWebApi apiHelper)
    {
        var scopes = new[] { "user.read" };

        try
        {
            var response = await apiHelper.CallWebApiForUserAsync(
                "GraphTest",
                o =>
                {
                    o.Scopes = string.Join(" ", scopes);
                    o.RelativePath = "v1.0/me";
                    o.HttpMethod = HttpMethod.Get;
                });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("givenName", out var givenNameProp))
            {
                result.GivenName = givenNameProp.GetString();
            }

            if (doc.RootElement.TryGetProperty("surname", out var surnameProp))
            {
                result.Surname = surnameProp.GetString();
            }
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
        {
            _logger.LogWarning("Additional action is required: " + ex.MsalUiRequiredException.Classification);
            //tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex.MsalUiRequiredException, HttpContext.Response);
        }
        catch (MsalUiRequiredException ex)
        {
            _logger.LogWarning("Additional action is required: " + ex.Classification);
            //tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex, HttpContext.Response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error calling the downstream API");
        }
    }
    
    private async Task FillTaskLists(ProfileData result, IDownstreamWebApi apiHelper)
    {
        var scopes = new[] { "Directory.Read.All" };

        try
        {
            var response = await apiHelper.CallWebApiForUserAsync(
                "GraphTest",
                o =>
                {
                    o.Scopes = string.Join(" ", scopes);
                    o.RelativePath = "v1.0/me/transitiveMemberOf/microsoft.graph.group";
                    o.HttpMethod = HttpMethod.Get;
                });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("value", out var valueCollection))
            {
                var items = new List<string>();
                foreach (var value in valueCollection.EnumerateArray())
                {
                    var name = value.GetProperty("displayName").GetString();
                    items.Add(name ?? "* Unnamed");
                }

                result.Groups = items.ToArray();
            }
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
        {
            _logger.LogWarning("Additional action is required: " + ex.MsalUiRequiredException.Classification);
            //tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex.MsalUiRequiredException, HttpContext.Response);
        }
        catch (MsalUiRequiredException ex)
        {
            _logger.LogWarning("Additional action is required: " + ex.Classification);
            //tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex, HttpContext.Response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error calling the downstream API");
        }
    }
}