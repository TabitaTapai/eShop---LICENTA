using eShop.Entities;
using eShop.Services;
using eShop.Shared;
using eShop.Shared.Enums;
using eShop.Shared.Extensions;
using eShop.Shared.Helpers;
using eShop.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eShop.Web.Controllers
{
    public class HomeController : PublicBaseController
    {
        public ActionResult Index()
        {         
            //facem redirectare catre limba default daca home page este accesata fara limba si daca este activata Multilingual true
            if(ConfigurationsHelper.EnableMultilingual && Request.Url.AbsolutePath.ToString().Equals("/"))
            {
                return Redirect(Url.Home());
            }

            return View(new PageViewModel());
        }

        //[OutputCache(Duration = 600)]
        public ActionResult HomeSliders()
        {
            HomeSlidersViewModel model = new HomeSlidersViewModel
            {
                SlidersConfigurations = ConfigurationsService.Instance.GetConfigurationsByType((int)ConfigurationTypes.Sliders)
            };

            return PartialView("_BannerSlider", model);
        }

        public ActionResult Search(string category, string q, decimal? from, decimal? to, string sortby, int? pageNo, int? recordSize)
        {
            recordSize = recordSize ?? (int)RecordSizeEnums.Size20;

            ProductsViewModel model = new ProductsViewModel
            {
                Categories = CategoriesService.Instance.GetCategories()
            };

            if (!string.IsNullOrEmpty(category))
            {
                var selectedCategory = CategoriesService.Instance.GetCategoryByName(category);

                if (selectedCategory == null) return HttpNotFound();
                else
                {
                    model.CategoryID = selectedCategory.ID;
                    model.CategoryName = selectedCategory.SanitizedName;
                    model.SelectedCategory = selectedCategory;

                    model.SearchedCategories = CategoryHelpers.GetAllCategoryChildrens(selectedCategory, model.Categories);
                }
            }
            
            model.SearchTerm = q;
            model.PriceFrom = from;
            model.PriceTo = to;
            model.SortBy = sortby;
            model.PageSize = recordSize;
            
            var selectedCategoryIDs = model.SearchedCategories != null ? model.SearchedCategories.Select(x => x.ID).ToList() : null;

            model.Products = ProductsService.Instance.SearchProducts(selectedCategoryIDs, model.SearchTerm, model.PriceFrom, model.PriceTo, model.SortBy, pageNo, recordSize.Value, activeOnly: true, out int count, stockCheckCount: null);

            model.Pager = new Pager(count, pageNo, recordSize.Value);

            return View(model);
        }

        public ActionResult PriceRangeFilter(decimal? priceFrom, decimal? priceTo)
        {
            var model = new PriceRangeFilterViewModel
            {
                PriceFrom = priceFrom,
                PriceTo = priceTo,

                MaxPrice = ProductsService.Instance.GetMaxProductPrice()
            };

            return PartialView("SearchFilters/_PriceRangeFilter", model);
        }

        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public JsonResult SubscribeNewsLetter(string email)
        {
            JsonResult jsonResult = new JsonResult();

            var newsletterSubscription = new NewsletterSubscription()
            {
                EmailAddress = email,
                IsVerified = false,
                UserID = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : string.Empty,
                ModifiedOn = DateTime.Now,
                IsActive = true
            };

            var result = SharedService.Instance.SaveNewsletterSubscription(newsletterSubscription);

            if(result)
            {
                jsonResult.Data = new
                {
                    Success = true,
                    Message = "PP.Footer.NewsLetter.NewsletterSubscribed".LocalizedString()
                };
            }
            else
            {
                jsonResult.Data = new
                {
                    Success = false,
                    Message = "PP.Footer.NewsLetter.NewsletterAlreadySubscribed".LocalizedString()
                };
            }

            return jsonResult;
        }

        // daca nu exista token sau este invalid, action method nu se executa
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SubmitContactForm(string subject, string name, string email, string message)
        {
            JsonResult jsonResult = new JsonResult();

            try
            {
                //trimitem mail de notificare comanda noua catre admin email
                await new EmailService()
                                 .SendToEmailAsync(ConfigurationsHelper.SendGrid_FromEmailAddressName,
                                                   ConfigurationsHelper.SendGrid_FromEmailAddress,
                                                   ConfigurationsHelper.AdminEmailAddress,
                                                   EmailTextHelpers.ContactMessageSubject_Admin(),
                                                   EmailTextHelpers.ContactMessageBody_Admin(subject, name, email, message));

                jsonResult.Data = new
                {
                    //daca s-a trimis cu succes afisam mesajul
                    Success = true,
                    Message = "Your message has been submitted. We will contact you back soon."
                };
            }
            catch (Exception)
            {
                jsonResult.Data = new
                {
                    //daca email-ul nu s-a trimis cu succes afisam mesajul
                    Success = false,
                    Message = "An error occured while submitting your message."
                };
            }

            return jsonResult;
        }
    }
}