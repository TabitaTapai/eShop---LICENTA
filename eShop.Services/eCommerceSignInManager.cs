using eShop.Entities;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Services
{
    public class eShopSignInManager : SignInManager<eShopUser, string>
    {
        public eShopSignInManager(eShopUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(eShopUser user)
        {
            return user.GenerateUserIdentityAsync((eShopUserManager)UserManager);
        }

        public static eShopSignInManager Create(IdentityFactoryOptions<eShopSignInManager> options, IOwinContext context)
        {
            return new eShopSignInManager(context.GetUserManager<eShopUserManager>(), context.Authentication);
        }
    }
}
