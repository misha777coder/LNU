using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WishlistWeb.Data;
using WishlistWeb.Models;
using WishlistWeb.Models.Wishlist;

namespace WishlistWeb.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public WishlistController(AppDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    private Guid UserId() => Guid.Parse(_users.GetUserId(User)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var uid = UserId();
        var items = await _db.WishlistItems
            .Where(x => x.UserId == uid)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(ItemVm vm)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

        var uid = UserId();
        _db.WishlistItems.Add(new WishlistItem
        {
            UserId = uid,
            Title = vm.Title,
            Url = string.IsNullOrWhiteSpace(vm.Url) ? null : vm.Url.Trim(),
            Note = string.IsNullOrWhiteSpace(vm.Note) ? null : vm.Note.Trim()
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Trash()
    {
        var uid = UserId();
        var items = await _db.TrashItems
            .Where(x => x.UserId == uid)
            .OrderByDescending(x => x.DeletedAtUtc)
            .ToListAsync();

        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var uid = UserId();
        var item = await _db.WishlistItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);
        if (item is null) return RedirectToAction(nameof(Index));

        _db.TrashItems.Add(new TrashItem
        {
            UserId = uid,
            Title = item.Title,
            Url = item.Url,
            Note = item.Note
        });

        _db.WishlistItems.Remove(item);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        var uid = UserId();
        var t = await _db.TrashItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);
        if (t is null) return RedirectToAction(nameof(Trash));

        _db.WishlistItems.Add(new WishlistItem
        {
            UserId = uid,
            Title = t.Title,
            Url = t.Url,
            Note = t.Note
        });

        _db.TrashItems.Remove(t);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Trash));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Purge(Guid id)
    {
        var uid = UserId();
        var t = await _db.TrashItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);
        if (t is null) return RedirectToAction(nameof(Trash));

        _db.TrashItems.Remove(t);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Trash));
    }
}
