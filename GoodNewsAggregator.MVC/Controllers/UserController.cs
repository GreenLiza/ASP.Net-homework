using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.DTO;
using GoodNewsAggregator.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace GoodNewsAggregator.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService,
            IRoleService roleService,
            IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
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
                    await Authenticate(user);
                    return RedirectToAction("Account", "User", new { user.Username });
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogIn(string returnUrl = null)
        {
            var model = new UserLogInModel()
            {
                ReturnUrl = returnUrl
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserLogInModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.LogInUserAsync(model.Username, model.Password);
                if (user != null)
                {
                    await Authenticate(user);
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return LocalRedirect(model.ReturnUrl);
                    }                   
                    return RedirectToAction("Account", "User", new { user.Username });
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

        
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LogIn", "User");
        }

        [Authorize]
        public async Task<IActionResult> Account([FromQuery] UserAccountSearchModel model)
        {
            var userDto = await _userService.GetUserByUsername(model.Username);
            if (userDto != null)
            {
                UserAccountModel userModel = _mapper.Map<UserAccountModel>(userDto);
                if (User.FindFirstValue(ClaimTypes.Role) == "Admin")
                {
                    return View("AdminAccount", userModel);
                }
                return View(userModel);
            }
            return Content($"User {model.Username} is not registed");
        }

        private async Task Authenticate(UserDto dto)
        {
            try
            {
                const string authType = "Application Cookies";
                var role = await _roleService.GetRoleNameById(await _userService.GetUserRoleId(dto.Username));
                if (string.IsNullOrEmpty(role))
                {
                    throw new ArgumentException("User data is incorrect");
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                var identity = new ClaimsIdentity(claims, authType,
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
