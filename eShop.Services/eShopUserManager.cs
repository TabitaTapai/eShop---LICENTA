using eShop.Data;
using eShop.Entities;
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
    public class eShopUserManager : UserManager<eShopUser>
    {
        public eShopUserManager(IUserStore<eShopUser> store)
            : base(store)
        {
        }

        public static eShopUserManager Create(IdentityFactoryOptions<eShopUserManager> options, IOwinContext context)
        {
            var manager = new eShopUserManager(new UserStore<eShopUser>(context.Get<eShopContext>()));

            // configurare logica de validare pentru nume utilizator
            manager.UserValidator = new UserValidator<eShopUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            // configurare logica de validare pentru parola
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // configurare valori implicite de blocare a utilizatorului
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Inregistrate autorizare in 2 pasi cu telefonul sau mail-ul
            
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<eShopUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<eShopUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<eShopUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
