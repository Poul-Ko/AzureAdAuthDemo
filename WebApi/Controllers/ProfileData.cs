namespace WebApi.Controllers;

public class ProfileData
{
    public string Id { get; init; } = string.Empty;
    public string? UserName { get; init; }
    public string? PreferredName { get; init; }
    public string? GivenNameFromGraph { get; set; }
    public string? SurnameFromGraph { get; set; }
    public string[]? GroupsFromGraph { get; set; }
}