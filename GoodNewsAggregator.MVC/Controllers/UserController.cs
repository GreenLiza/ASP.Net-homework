using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoodNewsAggregator.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService,
            IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.RegisterUserAsync(model.Email, model.Username, model.Password);
                if (user != null)
                {

                    return RedirectToAction("Account", new {user.Username, user.Email});
                }
                
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserLogInModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.LogInUserAsync(model.Username, model.Password);
                if (user != null)
                {
                    return RedirectToAction("Account", new { user.Username, user.Email});
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckIsEmailNotExist(string email)
        {
            return Ok(!await _userService.IsUserWithEmailExistAsync(email));
        }

        [HttpGet]
        public async Task<IActionResult> CheckIsUsernameNotExist(string username)
        {
            return Ok(!await _userService.IsUserWithUsernameExistAsync(username));
        }

        public IActionResult LogOut()
        {
            return RedirectToAction("LogIn", "User");
        }

        public IActionResult Account([FromQuery] UserAccountModel model)
        {
            return View(model);
        }
    }
}
