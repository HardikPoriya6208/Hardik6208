using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.BAL;
using MVC.Models;

namespace MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserHelper _userHelper;
    private readonly IWebHostEnvironment _env;

    public HomeController(ILogger<HomeController> logger, UserHelper userHelper, IWebHostEnvironment env)
    {
        _logger = logger;
        _userHelper = userHelper;
        _env = env;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        string role = HttpContext.Session.GetString("role");
        if (HttpContext.Session.GetString("role") != "User")
            return RedirectToAction("Login");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // --------------- Register -----------------

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(t_registeruser user)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Invalid Input..!" });

        if (user.ProfileImage != null)
        {
            string path = Path.Combine(_env.WebRootPath, "Image");
            Directory.CreateDirectory(path);

            string file = user.c_email + Path.GetExtension(user.ProfileImage.FileName);
            using var stream = new FileStream(Path.Combine(path, file), FileMode.Create);
            await user.ProfileImage.CopyToAsync(stream);

            user.c_image = file;
        }
        else
        {
            user.c_image = "default.png";
        }

        int status = await _userHelper.Register(user);

        return status switch
        {
            1 => Json(new { success = true, message = "Registration Successful" }),
            -1 => Json(new { success = false, message = "Email Already Exists!" }),
            _ => Json(new { success = false, message = "Error While Registering!" })
        };
    }


    // =============  LOGIN PAGE ============= 
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(vm_login data)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Invalid Login!" });

        var user = await _userHelper.Login(data.c_email, data.c_password);

        if (user == null)
            return Json(new { success = false, message = "Invalid Email or Password!" });

        // SESSION
        HttpContext.Session.SetInt32("c_userid", user.c_userid ?? 0);
        HttpContext.Session.SetString("role", user.c_role);

        // ROLE BASED REDIRECT
        string redirectUrl = user.c_role == "Admin"
            ? Url.Action("Index", "Admin")
            : Url.Action("Index", "Tour");

        return Json(new
        {
            success = true,
            message = "Login Successful",
            redirectUrl = redirectUrl
        });
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    // ---------- PROFILE ----------
    public async Task<IActionResult> Profile()
    {
        int? id = HttpContext.Session.GetInt32("c_userid");
        if (id == null) return RedirectToAction("Login");

        var user = await _userHelper.GetUserById(id.Value);
        return View(user);
    }

    // ---------- UPDATE PROFILE ----------
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(t_registeruser model)
    {
        int? id = HttpContext.Session.GetInt32("c_userid");
        if (id == null) return RedirectToAction("Login");

        model.c_userid = id.Value;

        if (await _userHelper.UpdateProfile(model))
            TempData["ProfileSuccess"] = "Profile Updated Successfully!";
        else
            TempData["ProfileError"] = "Profile Update Failed!";

        return RedirectToAction("Profile");
    }

    // ---------- UPDATE IMAGE ----------
    [HttpPost]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile ProfilePicture)
    {
        int? id = HttpContext.Session.GetInt32("c_userid");
        if (id == null) return RedirectToAction("Login");

        string folder = Path.Combine(_env.WebRootPath, "Image");
        Directory.CreateDirectory(folder);

        string filename = $"{id}_{DateTime.Now.Ticks}{Path.GetExtension(ProfilePicture.FileName)}";
        string path = Path.Combine(folder, filename);

        using var fs = new FileStream(path, FileMode.Create);
        await ProfilePicture.CopyToAsync(fs);

        await _userHelper.UpdateProfilePicture(id.Value, filename);

        TempData["ProfileSuccess"] = "Profile Image Updated!";
        return RedirectToAction("Profile");
    }

    // ---------- UPDATE PASSWORD ----------
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            TempData["PasswordError"] = "Passwords do not match!";
            return RedirectToAction("Profile");
        }

        int id = HttpContext.Session.GetInt32("c_userid") ?? 0;

        int result = await _userHelper.UpdatePassword(id, currentPassword, newPassword);

        TempData[result == 1 ? "PasswordSuccess" : "PasswordError"] =
            result == 1 ? "Password Updated!" : "Wrong Current Password!";

        return RedirectToAction("Profile");
    }

}
