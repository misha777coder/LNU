using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishlistWeb.Models;
using WishlistWeb.Models.Auth;


namespace WishlistWeb.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;

    public AccountController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => 
        View(new WishlistWeb.Models.Auth.LoginVm { ReturnUrl = returnUrl });


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _users.FindByEmailAsync(vm.Email);
        if (user is null)
        {
            ModelState.AddModelError("", "faild email");
            return View(vm);
        }

        var result = await _signIn.PasswordSignInAsync(user, vm.Password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "faild email");
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
            return Redirect(vm.ReturnUrl);

        return RedirectToAction("Index", "Wishlist");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
        var res = await _users.CreateAsync(user, vm.Password);

        if (!res.Succeeded)
        {
            foreach (var e in res.Errors) ModelState.AddModelError("", e.Description);
            return View(vm);
        }

        await _signIn.SignInAsync(user, isPersistent: true);
        return RedirectToAction("Index", "Wishlist");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Login");
    }
}
