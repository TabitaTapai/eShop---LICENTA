using eShop.Entities;
using eShop.Web.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eShop.Web.Areas.Dashboard.ViewModels
{
    public class RolesListingViewModel : PageViewModel
    {
        public string SearchTerm { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public Pager Pager { get; set; }
    }

    public class RoleDetailsViewModel : PageViewModel
    {
        public IdentityRole Role { get; set; }

        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class RoleUsersViewModel : PageViewModel
    {
        public List<eShopUser> RoleUsers { get; set; }

        public Pager Pager { get; set; }
        public string RoleID { get; internal set; }
    }

    public class UserRolesViewModel : PageViewModel
    {
        public List<IdentityRole> AvailableRoles { get; set; }
        public List<IdentityRole> UserRoles { get; set; }
        public eShopUser User { get; internal set; }
    }
}