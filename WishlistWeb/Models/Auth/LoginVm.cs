using System.ComponentModel.DataAnnotations;

namespace WishlistWeb.Models.Auth;

public class LoginVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}