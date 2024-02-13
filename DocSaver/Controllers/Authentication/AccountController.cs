using DocSaver.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DocSaver.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager
            , SignInManager<IdentityUser> signinManager
            , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chkEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (chkEmail != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email already exist!");
                        return View(model);
                    }

                    var user = new IdentityUser()
                    {
                        UserName = model.Username,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        RedirectToAction("MapUserRole", "AppRoles");

                        await _signinManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            try
            {
                IdentityUser chkUser;
                if (model.UsernameOrEmail.IndexOf('@') > -1)
                {
                    chkUser = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                }
                else
                {
                    chkUser = await _userManager.FindByNameAsync(model.UsernameOrEmail);
                }

                if (chkUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Incorrect Email/Username!");
                    return View(model);
                }

                if (await _userManager.CheckPasswordAsync(chkUser, model.Password) == false)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Credentials!");
                    return View(model);
                }
                var result = await _signinManager.PasswordSignInAsync(chkUser, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(AddRole model)
        {
            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
            }
            return RedirectToAction("GetRoles");
        }
    }
}
