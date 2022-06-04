using eShop.Entities;
using eShop.Services;
using eShop.Shared.Extensions;
using eShop.Shared.Helpers;
using eShop.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eShop.Web.Controllers
{
    [Authorize]
    public class UsersController : PublicBaseController
    {
        #region Constructor and Managers
        private eShopSignInManager _signInManager;
        private eShopUserManager _userManager;
        private eShopRoleManager _roleManager;

        public eShopSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<eShopSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public eShopUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<eShopUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public eShopRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<eShopRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // constructor UsersController()
        public UsersController()
        {
        }

        public UsersController(eShopUserManager userManager, eShopSignInManager signInManager, eShopRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        #endregion

        // *********** inregistratre utilizator ***********

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 0)]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        // acces utilizatorilor neautentificati pentru actiuni individuale
        [AllowAnonymous]
        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            JsonResult jsonResult = new JsonResult();

            var user = new eShopUser { FullName = model.FullName, UserName = model.Username, Email = model.Email, RegisteredOn = DateTime.Now };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (await RoleManager.RoleExistsAsync("User"))
                {
                    // asignare rol pentru noul user
                    await UserManager.AddToRoleAsync(user.Id, "User");
                }

                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");                

                await UserManager.SendEmailAsync(user.Id, EmailTextHelpers.AccountRegisterEmailSubject(AppDataHelper.CurrentLanguage.ID), EmailTextHelpers.AccountRegisterEmailBody(AppDataHelper.CurrentLanguage.ID, Url.Action("Login", "Users", null, protocol: Request.Url.Scheme)));

                jsonResult.Data = new { Success = true };
            }
            else
            {
                jsonResult.Data = new { Success = false, Messages = string.Join("<br />", result.Errors) };
            }

            return jsonResult;
        }
        
        // ********** autentificare ***********

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 0)]
        public ActionResult Login(string returnUrl)
        {
            // utilizatorul incepe logarea se invoca o noua sesiune de login 
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        // acces utilizatorilor neautentificati pentru actiuni individuale
        [AllowAnonymous]
        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            JsonResult jsonResult = new JsonResult();

            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    jsonResult.Data = new { Success = true, RequiresVerification = false };
                    break;
                case SignInStatus.RequiresVerification:
                    jsonResult.Data = new { Success = true, RequiresVerification = true };
                    break;
                case SignInStatus.LockedOut:
                    jsonResult.Data = new { Success = false, Messages = "PP.Login.Validation.LockedOut".LocalizedString() };
                    break;
                case SignInStatus.Failure:
                default:
                    jsonResult.Data = new { Success = false, Messages = "PP.Login.Validation.InvalidLoginAttempt".LocalizedString() };
                    break;
            }

            return jsonResult;
        }

        // *********** logare cu conturi sociale ***********

        [HttpPost]
        // acces utilizatorilor neautentificati pentru actiuni individuale
        [AllowAnonymous]
        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public ActionResult SocialLogin(string provider, string returnUrl)
        {
            // se solicita redirectionare catre furnizor extern de conectare eg. Facebook
            return new ChallengeResult(provider, Url.SocialLoginCallback());
        }

        [AllowAnonymous]
        public async Task<ActionResult> SocialLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return Redirect(Url.Login(returnUrl));
            }

            // logare utilizator cu datele de logare de Facebook
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Redirect(Url.Home());
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // daca user-ul nu are cont atunci creaza unul
                    var user = new eShopUser { UserName = loginInfo.DefaultUserName, Email = loginInfo.Email };
                    var createUserResult = await UserManager.CreateAsync(user);
                    if (createUserResult.Succeeded)
                    {
                        createUserResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                        if (createUserResult.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return Redirect(Url.Login(returnUrl));
                        }
                        else return View("Error");
                    }
                    else return View("Error");
            }
        }

        // *********** delogare ***********

        [HttpPost]
        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Redirect(Url.Home());
        }

        // acces utilizatorilor neautentificati pentru actiuni individuale
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        // *********** recuperare parola - generare token de resetare ***********

        [HttpPost]
        // acces utilizatorilor neautentificati pentru actiuni individuale
        [AllowAnonymous]
        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = !string.IsNullOrEmpty(model.Username) ? await UserManager.FindByNameAsync(model.Username) : null;

            if (user != null)
            {
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Action("ResetPassword", "Users", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await UserManager.SendEmailAsync(user.Id, "Reset " + ConfigurationsHelper.ApplicationName + " Password", "Please reset your " + ConfigurationsHelper.ApplicationName + " password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }

            // nu vom afisa deoarece userul inca nu exista, nu a confirmat
            // dar afisam un mesaj de succes

            JsonResult jResult = new JsonResult
            {
                Data = new { Success = true }
            };
            return jResult;
        }

        // *********** recuperare parola - resetare parola ***********
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Code = code,
                UserId = userId
            };

            return string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId) ? View("Error") : View(model);
        }

        [HttpPost]
        // acces utilizatorilor neautentificati pentru actiuni individuale
        [AllowAnonymous]
        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(ResetPasswordViewModel model)
        {
            JsonResult jResult = new JsonResult();

            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

                if (!result.Succeeded)
                {
                    jResult.Data = new { Success = result.Succeeded, Messages = string.Join("\n", result.Errors) };
                }
                else
                {
                    jResult.Data = new { Success = result.Succeeded, Messages = "Your password has been reset. Please login with your updated credentials now." };
                }
            }
            else
            {
                jResult.Data = new { Success = false, Messages = "Unable to reset password." };
            }

            return jResult;
        }

        // *********** profil utilizator ***********
        public ActionResult UserProfile(string tab)
        {
            ProfileDetailsViewModel model = new ProfileDetailsViewModel
            {
                ActiveTab = tab,

                User = UserManager.FindById(User.Identity.GetUserId())
            };

            if (model.User == null) return HttpNotFound();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserProfile", model);
            }
            else
            {
                return View(model);
            }
        }

        // *********** modificare profil utilizator ***********

        [HttpPost]
        public async Task<JsonResult> UpdateProfile(UpdateProfileDetailsViewModel model)
        {
            JsonResult jResult = new JsonResult();

            if (model != null && User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.UserName = model.Username;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Country = model.Country;
                    user.City = model.City;
                    user.Address = model.Address;
                    user.ZipCode = model.ZipCode;

                    var result = await UserManager.UpdateAsync(user);

                    jResult.Data = new { Success = result.Succeeded, Message = string.Join("\n", result.Errors) };

                    return jResult;
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }

        public async Task<ActionResult> ChangePassword()
        {
            ProfileDetailsViewModel model = new ProfileDetailsViewModel
            {
                User = await UserManager.FindByIdAsync(User.Identity.GetUserId())
            };

            return PartialView("_ChangePassword", model);
        }

        // *********** modificare parola ***********
        public async Task<JsonResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            JsonResult jResult = new JsonResult();

            if (model != null && User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user != null)
                {
                    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

                    jResult.Data = new { Success = result.Succeeded, Message = string.Join("\n", result.Errors) };

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }
        // *********** modificare poza profil ***********
        public async Task<ActionResult> ChangeAvatar()
        {
            ProfileDetailsViewModel model = new ProfileDetailsViewModel
            {
                User = await UserManager.FindByIdAsync(User.Identity.GetUserId())
            };

            return PartialView("_ChangeAvatar", model);
        }

        public async Task<JsonResult> UpdateAvatar(int pictureID)
        {
            JsonResult jResult = new JsonResult();

            if (pictureID > 0 && User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user != null)
                {
                    user.PictureID = pictureID;

                    var result = await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    jResult.Data = new { Success = result.Succeeded, Message = string.Join("\n", result.Errors) };
                }
            }
            else
            {
                jResult.Data = new { Success = false, Message = "Invalid User" };
            }

            return jResult;
        }

        // protectie XSRF pentru logari cu conturi sociale
        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}