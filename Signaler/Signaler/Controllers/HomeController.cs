using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Signaler.Models;
using Signaler.Models.Home;
using Signaler.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Signaler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Chats");
            }

            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Chats");
            }

            if (ModelState.IsValid)
            {
                if (_userService.UserExists(loginViewModel.Username, loginViewModel.Password))
                {
                    var user = _userService.GetUser(loginViewModel.Username);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Username),
                        new Claim(ClaimTypes.Role, "User")
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties {IsPersistent = false });

                    return Redirect("/Chats");
                }else
                {
                    ViewData["ErrorMessage"] = "Invalid username and/or password.";
                }
            }

            return View("Index");
        }

        [HttpGet]
        [Route("Home/Register")]
        public IActionResult RegisterGet()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Chats");
            }

            return View("Register");
        }

        [HttpPost]
        [Route("Home/Register")]
        public IActionResult RegisterPost(string username, string password)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Chats");
            }

            try
            {
                _userService.RegisterUser(username, password);

            }catch(UserExistsException _)
            {
                Console.WriteLine($"{username} already exists");
                ViewData["ErrorMessage"] = $"{username} already exists";
                return View("Register");
            }

            return View("Index");
        }

        [HttpGet]
        [Route("Home/Logout")]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
