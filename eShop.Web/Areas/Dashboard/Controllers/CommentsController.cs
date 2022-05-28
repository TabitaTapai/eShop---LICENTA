using eShop.Entities;
using eShop.Services;
using eShop.Web.Areas.Dashboard.ViewModels;
using eShop.Shared.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eShop.Shared.Enums;
using eShop.Web.ViewModels;
using System.Threading;

namespace eShop.Web.Areas.Dashboard.Controllers
{
    public class CommentsController : DashboardBaseController
    {
        private eShopUserManager _userManager;
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

        public async Task<ActionResult> Index(string userID, string searchTerm, int? pageNo, int entityID = (int)EntityEnums.Product, bool showUserCommentsOnly = false)
        {
            CommentsListingViewModel model = new CommentsListingViewModel
            {
                SearchTerm = searchTerm
            };

            if (!string.IsNullOrEmpty(userID))
            {
                model.User = await UserManager.FindByIdAsync(userID);
            }

            model.Comments = CommentsService.Instance.SearchComments(entityID: entityID, recordID: null, userID: userID, searchTerm: model.SearchTerm, pageNo: pageNo, recordSize: (int)RecordSizeEnums.Size10, count: out int commentsCount);

            if (model.Comments != null && model.Comments.Count > 0)
            {
                var productIDs = model.Comments.Select(x => x.RecordID).ToList();

                model.CommentedProducts = ProductsService.Instance.GetProductsByIDs(productIDs);
            }

            model.Pager = new Pager(commentsCount, pageNo, (int)RecordSizeEnums.Size10);

            if(showUserCommentsOnly)
            {
                return PartialView("_UserComments", model);
            }
            else return View(model);
        }        

        [HttpPost]
        public JsonResult Delete(int ID)
        {
            JsonResult result = new JsonResult();

            try
            {
                var comment = CommentsService.Instance.GetCommentByID(ID);

                if (comment != null && User.Identity.IsAuthenticated && (User.IsInRole("Administrator") || comment.UserID == User.Identity.GetUserId()))
                {
                    var operation = CommentsService.Instance.DeleteComment(comment);

                    result.Data = new { Success = operation, Message = operation ? string.Empty : "Dashboard.Comments.Action.Validation.UnableToDeleteComment".LocalizedString() };
                }
                else
                {
                    throw new Exception("Dashboard.Comments.Action.Validation.NotAuthorizedToDeleteComment".LocalizedString());
                }
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = string.Format("{0}", ex.Message) };
            }

            return result;
        }
    }
}