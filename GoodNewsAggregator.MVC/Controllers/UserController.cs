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
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(IUserService userService,
            IRoleService roleService, ILogger<UserController> logger,
            IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            _logger.LogInformation("Registration page is called");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Registration information passed validation");
                    var user = await _userService.RegisterUserAsync(model.Email, model.Username, model.Password);
                    if (user != null)
                    {
                        await Authenticate(user);
                        _logger.LogInformation("Registration success");
                        return RedirectToAction("Account", "User", new { user.Username });
                    }
                    _logger.LogInformation("Registration information was already used");
                }
                _logger.LogInformation("Registration information failed validation");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LogIn(string returnUrl = null)
        {
            _logger.LogInformation("LogIn page is called");

            var model = new UserLogInModel()
            {
                ReturnUrl = returnUrl
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserLogInModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("LogIn information passed validation");
                    var user = await _userService.LogInUserAsync(model.Username, model.Password);
                    if (user != null)
                    {
                        await Authenticate(user);
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }
                        _logger.LogInformation("LogIn success");
                        return RedirectToAction("Account", "User", new { user.Username });
                    }
                    _logger.LogInformation("User with specified logIn information doesn't exist");

                }
                _logger.LogInformation("LogIn information failed validation");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
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
            _logger.LogInformation("LogOut is called");
            await HttpContext.SignOutAsync();
            return RedirectToAction("LogIn", "User");
        }

        [Authorize]
        public async Task<IActionResult> Account([FromQuery] UserAccountSearchModel model)
        {
            _logger.LogInformation("Account page is called");
            try
            {
                var userDto = await _userService.GetUserByUsername(model.Username);
                if (userDto != null)
                {
                    UserAccountModel userModel = _mapper.Map<UserAccountModel>(userDto);
                    if (User.FindFirstValue(ClaimTypes.Role) == "Admin")
                    {
                        _logger.LogInformation("Admin account page is called");
                        return View("AdminAccount", userModel);
                    }
                    _logger.LogInformation("User account page is called");
                    return View(userModel);
                }
                _logger.LogInformation("User with specified parameters doesn't exist");
                return RedirectToAction("LogIn", "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
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
