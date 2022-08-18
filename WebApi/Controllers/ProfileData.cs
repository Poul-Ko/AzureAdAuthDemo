namespace WebApi.Controllers;

public class ProfileData
{
    public string Id { get; init; } = string.Empty;
    public string? UserName { get; init; }
    public string? PreferredName { get; init; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string[]? Groups { get; set; }
}