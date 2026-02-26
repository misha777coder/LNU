namespace WishlistWeb.Models;

public class TrashItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Title { get; set; } = "";
    public string? Url { get; set; }
    public string? Note { get; set; }

    public DateTime DeletedAtUtc { get; set; } = DateTime.UtcNow;
}