using System.ComponentModel.DataAnnotations;

namespace WishlistWeb.Models.Auth;

public class RegisterVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(6)]
    public string Password { get; set; } = "";

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";
}