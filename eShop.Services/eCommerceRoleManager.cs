using eShop.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Services
{
    public class eShopRoleManager : RoleManager<IdentityRole>
    {
        public eShopRoleManager(IRoleStore<IdentityRole, string> roleStore) : base(roleStore)
        {
        }

        public static eShopRoleManager Create(IdentityFactoryOptions<eShopRoleManager> options, IOwinContext context)
        {
            return new eShopRoleManager(new RoleStore<IdentityRole>(context.Get<eShopContext>()));
        }
    }
}
