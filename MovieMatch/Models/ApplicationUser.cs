using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MovieMatch.Models;

public class ApplicationUser : IdentityUser
{
    public string Nick { get; set; } = string.Empty;

    public string? StreamingPlatforms { get; set; }
    public ICollection<UserTitle> UserTitles { get; set; } = new List<UserTitle>();
    public ICollection<MovieComment> MovieComments { get; set; } = new List<MovieComment>();

}
