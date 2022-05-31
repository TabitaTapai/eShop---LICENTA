using eShop.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Data
{
    public class eShopDBInitializer : CreateDatabaseIfNotExists<eShopContext>
    {
        public int EnglishLanguageID { get; set; }
        public int RomanianLanguageID { get; set; }

        protected override void Seed(eShopContext context)
        {
            SeedLanguages(context);
            SeedRoles(context);
            SeedUsers(context);

            SeedCategories(context);
            SeedConfigurations(context);
        }

        public void SeedLanguages(eShopContext context)
        {
            Language english = new Language()
            {
                Name = "English",
                ShortCode = "en",
                IsEnabled = true,
                IsDefault = false,
                IconCode = "GB.png"
            };

            Language romanian = new Language()
            {
                Name = "Romana",
                ShortCode = "ro",
                IsEnabled = true,
                IsDefault = true,
                IsRTL = true,
                IconCode = "RO.png"
            };

            context.Languages.AddRange(new List<Language> { english, romanian });

            context.SaveChanges();

            EnglishLanguageID = english.ID;
            RomanianLanguageID = romanian.ID;
        }

        public void SeedRoles(eShopContext context)
        {
            List<IdentityRole> rolesInDealDouble = new List<IdentityRole>();

            rolesInDealDouble.Add(new IdentityRole() { Name = "Administrator" });
            rolesInDealDouble.Add(new IdentityRole() { Name = "Moderator" });
            rolesInDealDouble.Add(new IdentityRole() { Name = "User" });

            var rolesStore = new RoleStore<IdentityRole>(context);
            var rolesManager = new RoleManager<IdentityRole>(rolesStore);

            foreach (IdentityRole role in rolesInDealDouble)
            {
                if (!rolesManager.RoleExists(role.Name))
                {
                    var result = rolesManager.Create(role);

                    if (result.Succeeded) continue;
                }
            }
        }

        public void SeedUsers(eShopContext context)
        {
            var usersStore = new UserStore<eShopUser>(context);
            var usersManager = new UserManager<eShopUser>(usersStore);

            eShopUser admin = new eShopUser();
            admin.FullName = "Admin";
            
            admin.Email = "adm_use@domain.com";
            admin.UserName = "adm_use";
            var password = "adm_use123";

            admin.PhoneNumber = "(+40)770207660";
            admin.Country = "Romania";
            admin.City = "Timisoara";
            admin.Address = "Cutezatorilor 1";
            admin.ZipCode = "300088";

            admin.RegisteredOn = DateTime.Now;

            if (usersManager.FindByEmail(admin.Email) == null)
            {
                var result = usersManager.Create(admin, password);

                if (result.Succeeded)
                {
                    //se adauga rolurile necesare la admin
                    usersManager.AddToRole(admin.Id, "Administrator");
                    usersManager.AddToRole(admin.Id, "Moderator");
                    usersManager.AddToRole(admin.Id, "User");
                }
            }
        }
        
        public void SeedCategories(eShopContext context)
        {
            Category uncategorized = new Category()
            {
                SanitizedName = "uncategorized",
                DisplaySeqNo = 0,
                ModifiedOn = DateTime.Now
            };

            CategoryRecord uncategorizedEnRecord = new CategoryRecord()
            {
                Category = uncategorized,
                LanguageID = EnglishLanguageID, //utilizare globala
                Name = "Uncategorized",
                Description = "Products that are not categorized. uncategorised, unclassified - not arranged in any specific grouping.",
                ModifiedOn = DateTime.Now
            };

            CategoryRecord uncategorizedArRecord = new CategoryRecord()
            {
                Category = uncategorized,
                LanguageID = RomanianLanguageID, //utilizare globala
                Name = "Fara categorie",
                Description = "Produse care nu sunt clasificate. neclasificat, neclasificat - nu sunt în nicio grupare specifică.",
                ModifiedOn = DateTime.Now
            };

            context.Categories.Add(uncategorized);
            context.CategoryRecords.Add(uncategorizedEnRecord);
            context.CategoryRecords.Add(uncategorizedArRecord);

            context.SaveChanges();
        }

        public void SeedConfigurations(eShopContext context)
        {
            Configuration ApplicationName = new Configuration()
            {
                Key = "ApplicationName",
                Value = "eShop",
                Description = "This is the application name that will be used accross the site.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration ApplicationIntro = new Configuration()
            {
                Key = "ApplicationIntro",
                Value = "eShop - Lucrare de licenta.",
                Description = "Adaugati descrierea aplicatiei. Acesta va fi afisata in footer.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration AddressLine1 = new Configuration()
            {
                Key = "AddressLine1",
                Value = "",
                Description = "Add Address Line 1 of your company.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration AddressLine2 = new Configuration()
            {
                Key = "AddressLine2",
                Value = "",
                Description = "Add Address Line 2 of your company.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration PhoneNumber = new Configuration()
            {
                Key = "PhoneNumber",
                Value = "",
                Description = "Add Phone Number of your company.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration MobileNumber = new Configuration()
            {
                Key = "MobileNumber",
                Value = "",
                Description = "Add Mobile Number of your company.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration Email = new Configuration()
            {
                Key = "Email",
                Value = "contact@eshop.ro",
                Description = "Add email address of your company.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration AdminEmailAddress = new Configuration()
            {
                Key = "AdminEmailAddress",
                Value = "tabita.tapai@gmail.com",
                Description = "Add email address on which you want to receive notifications related to your store.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration FacebookURL = new Configuration()
            {
                Key = "FacebookURL",
                Value = "",
                Description = "Add Facebook Page URL of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration TwitterUsername = new Configuration()
            {
                Key = "TwitterUsername",
                Value = "",
                Description = "Add Twitter Username of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration TwitterURL = new Configuration()
            {
                Key = "TwitterURL",
                Value = "",
                Description = "Add Twitter Page URL of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration WhatsAppNumber = new Configuration()
            {
                Key = "WhatsAppNumber",
                Value = "",
                Description = "Add WhatsApp number of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration InstagramURL = new Configuration()
            {
                Key = "InstagramURL",
                Value = "",
                Description = "Add Instagram Page URL of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration YouTubeURL = new Configuration()
            {
                Key = "YouTubeURL",
                Value = "",
                Description = "Add YouTube Page URL of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration LinkedInURL = new Configuration()
            {
                Key = "LinkedInURL",
                Value = "",
                Description = "Add LinkedIn Page URL of your company.",
                ConfigurationType = (int)ConfigurationTypes.SocialLinks,
                ModifiedOn = DateTime.Now
            };

            Configuration Slider1 = new Configuration()
            {
                Key = "Slider1",
                Value = "#IMG#site/sliders/slider1.jpg#TH#Welcome to eShop#PG#eShop.#LK#/en/search",
                Description = "This is the first slider that will be displayed on the home page.",
                ConfigurationType = (int)ConfigurationTypes.Sliders,
                ModifiedOn = DateTime.Now
            };

            Configuration Slider2 = new Configuration()
            {
                Key = "Slider2",
                Value = "#IMG#site/sliders/slider2.jpg#TH#Welcome to eShop#PG#eShop.#LK#/en/search",
                Description = "This is the second slider that will be displayed on the home page.",
                ConfigurationType = (int)ConfigurationTypes.Sliders,
                ModifiedOn = DateTime.Now
            };

            Configuration Slider3 = new Configuration()
            {
                Key = "Slider3",
                Value = "#IMG#site/sliders/slider3.jpg#TH#Welcome to eShop#PG#eShop.#LK#/en/search",
                Description = "This is the third slider that will be displayed on the home page.",
                ConfigurationType = (int)ConfigurationTypes.Sliders,
                ModifiedOn = DateTime.Now
            };

            Configuration CurrencySymbol = new Configuration()
            {
                Key = "CurrencySymbol",
                Value = "Lei",
                Description = "This currency symbol is shown with prices on website and invoices.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration PriceCurrencyPosition = new Configuration()
            {
                Key = "PriceCurrencyPosition",
                Value = "{price}{currency}",
                Description = "This configuration will set price and currency relation accross the website. {price} will be replaced with the price value and {currency} will be replaced by configured currency symbol.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration DigitsAfterDecimalPoint = new Configuration()
            {
                Key = "DigitsAfterDecimalPoint",
                Value = "2",
                Description = "Set the value to any number to display number of digits after decimal points in amounts accross the site.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration EnableCreditCardPayment = new Configuration()
            {
                Key = "EnableCreditCardPayment",
                Value = "true",
                Description = "Set value to true to enable Credit card payments or set value to false to disable credit card payments.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration EnableCashOnDeliveryMethod = new Configuration()
            {
                Key = "EnableCashOnDeliveryMethod",
                Value = "true",
                Description = "Set value to true to enable Cash on Delivery Method or set value to false to disable Cash on Delivery Method.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration EnablePayPalPaymentMethod = new Configuration()
            {
                Key = "EnablePayPalPaymentMethod",
                Value = "false",
                Description = "Set value to true to enable PayPal Payment Method or set value to false to disable PayPal Method. You should also supply PayPal client id in order for paypal payment gateway to work.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration PayPalClientID = new Configuration()
            {
                Key = "PayPalClientID",
                Value = "",
                Description = "Add your PayPal client ID to use PayPal Payment Method. You should also enable PayPal Payment Method in order for paypal payment gateway to work.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration FlatDeliveryCharges = new Configuration()
            {
                Key = "FlatDeliveryCharges",
                Value = "0",
                Description = "Set the value for Delivery Charges. This is flat delivery charges rate.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration EnableMultilingual = new Configuration()
            {
                Key = "EnableMultilingual",
                Value = "true",
                Description = "Set value to true to enable Multi Language Features or set value to false to disable Multi Language.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration AuthorizeNet_ApiLoginID = new Configuration()
            {
                Key = "AuthorizeNet_ApiLoginID",
                Value = "",
                Description = "This is ApiLoginID which is used for logging in to Authorize .Net API.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration AuthorizeNet_ApiTransactionKey = new Configuration()
            {
                Key = "AuthorizeNet_ApiTransactionKey",
                Value = "",
                Description = "This is ApiTransactionKey which is used in Authorize .Net API Calls.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration AuthorizeNet_SandBox = new Configuration()
            {
                Key = "AuthorizeNet_SandBox",
                Value = "true",
                Description = "Set value to true to enable AuthorizeNet in SandBox or set value to false to disable AuthorizeNet in SandBox.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration SendGrid_APIKey = new Configuration()
            {
                Key = "SendGrid_APIKey",
                Value = "",
                Description = "Send Grid API Key used to connect to SendGrid services to send emails.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration SendGrid_FromEmailAddress = new Configuration()
            {
                Key = "SendGrid_FromEmailAddress",
                Value = "tabita.tapai@gmail.com",
                Description = "This is the email address that is displayed in email header to users when they recive Email via SendGrid Email.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration SendGrid_FromEmailAddressName = new Configuration()
            {
                Key = "SendGrid_FromEmailAddressName",
                Value = "eShop Team",
                Description = "This name is displayed on Email sent via SendGrid Email.",
                ConfigurationType = (int)ConfigurationTypes.Site,
                ModifiedOn = DateTime.Now
            };

            Configuration GoogleAnalyticsScript = new Configuration()
            {
                Key = "GoogleAnalyticsScript",
                Value = "",
                Description = "Add Google Analytics script code for site traffic analysis.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            Configuration FacebookMessengerScript = new Configuration()
            {
                Key = "FacebookMessengerScript",
                Value = "",
                Description = "Add Facebook Messenger script code for messenger chat.",
                ConfigurationType = (int)ConfigurationTypes.Other,
                ModifiedOn = DateTime.Now
            };

            context.Configurations.AddRange(new List<Configuration> { ApplicationName, ApplicationIntro, AddressLine1, AddressLine2, PhoneNumber, MobileNumber, Email, AdminEmailAddress, FacebookURL, TwitterUsername, TwitterURL, WhatsAppNumber, InstagramURL, YouTubeURL, LinkedInURL, Slider1, Slider2, Slider3, CurrencySymbol, PriceCurrencyPosition, DigitsAfterDecimalPoint, EnableCreditCardPayment, EnableCashOnDeliveryMethod, EnablePayPalPaymentMethod, PayPalClientID, FlatDeliveryCharges, EnableMultilingual, AuthorizeNet_ApiLoginID, AuthorizeNet_ApiTransactionKey, AuthorizeNet_SandBox, SendGrid_APIKey, SendGrid_FromEmailAddress, SendGrid_FromEmailAddressName, GoogleAnalyticsScript, FacebookMessengerScript });

            context.SaveChanges();
        }
    }
}
