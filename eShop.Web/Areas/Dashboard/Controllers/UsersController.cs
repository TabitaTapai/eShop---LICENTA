using eShop.Services;
using eShop.Web.Areas.Dashboard.ViewModels;
using eShop.Web.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using eShop.Shared.Helpers;
using eShop.Shared.Enums;

namespace eShop.Web.Areas.Dashboard.Controllers
{
    public class UsersController : DashboardBaseController
    {
        private eShopUserManager _userManager;
        private eShopRoleManager _roleManager;
        private eShopSignInManager _signInManager;

        public UsersController()
        {
        }

        public UsersController(eShopUserManager userManager, eShopRoleManager roleManager, eShopSignInManager signInManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }

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

        public ActionResult Index(string roleID, string searchTerm, int? pageNo)
        {
            var pageSize = (int)RecordSizeEnums.Size10;

            UsersListingViewModel model = new UsersListingViewModel
            {
                RoleID = roleID,
                SearchTerm = searchTerm,

                Roles = RoleManager.Roles.ToList()
            };

            var users = UserManager.Users;

            if (!string.IsNullOrEmpty(roleID))
            {
                users = users.Where(x => x.Roles.FirstOrDefault(y => y.RoleId == roleID) != null);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(x => x.Email.ToLower().Contains(searchTerm.ToLower()) || x.UserName.ToLower().Contains(searchTerm.ToLower()));
            }

            pageNo = pageNo ?? 1;
            var skipCount = (pageNo.Value - 1) * pageSize;

            model.Users = users.OrderByDescending(x => x.RegisteredOn).Skip(skipCount).Take(pageSize).ToList();

            model.Pager = new Pager(users.Count(), pageNo, pageSize);

            return View(model);
        }

        public async Task<ActionResult> UserDetails(string userID, bool isPartial = false)
        {
            UserDetailsViewModel model = new UserDetailsViewModel();

            var user = await UserManager.FindByIdAsync(userID);

            if (user != null)
            {
                model.User = user;
            }

            if (isPartial || Request.IsAjaxRequest())
            {
                return PartialView("_UserDetails", model);
            }
            else
            {
                return View(model);
            }
        }
        
        [HttpPost]
        public async Task<JsonResult> Action(UserDetailsViewModel model)
        {
            JsonResult json = new JsonResult();

            try
            {
                if (model != null)
                {
                    var user = await UserManager.FindByIdAsync(model.ID);

                    if (user != null)
                    {
                        user.FullName = model.FullName;
                        user.Country = model.Country;
                        user.City = model.City;
                        user.Address = model.Address;
                        user.ZipCode = model.ZipCode;

                        var result = await UserManager.UpdateAsync(user);

                        if(result.Succeeded)
                        {
                            json.Data = new { Success = result.Succeeded, Message = "Dashboard.UserDetails.Info.Action.Validation.InfoUpdated".LocalizedString() };
                        }
                        else
                        {
                            throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UnabletoUpdateInfo".LocalizedString());
                        }
                    }
                    else
                    {
                        throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UserNotFound".LocalizedString());
                    }
                }
                else
                {
                    throw new Exception("Dashboard.UserDetails.Info.Action.Validation.DataNotFormatted".LocalizedString());
                }
            }
            catch (Exception ex)
            {
                json.Data = new { Success = false, Message = ex.Message };
            }

            return json;
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string userID)
        {
            JsonResult json = new JsonResult();

            try
            {
                var user = await UserManager.FindByIdAsync(userID);

                if (user != null)
                {
                    var result = await UserManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        json.Data = new { Success = result.Succeeded, Message = "Dashboard.UserDetails.Info.Action.Validation.UserDeleted".LocalizedString() };
                    }
                    else
                    {
                        throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UnabletoDeleteUser".LocalizedString());
                    }
                }
                else
                {
                    throw new Exception("Dashboard.UserDetails.Info.Action.Validation.UserNotFound".LocalizedString());
                }
            }
            catch (Exception ex)
            {
                json.Data = new { Success = false, Message = ex.Message };
            }

            return json;
        }
    }
}