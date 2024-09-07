using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Endpoints;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Logout()
    {   
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Console.WriteLine("User has been logged out!");
        return RedirectToPage("/");

    }
}
