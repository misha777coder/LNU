using System.ComponentModel.DataAnnotations;

namespace WishlistWeb.Models.Wishlist;

public class ItemVm
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string? Url { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; }
}