using eShop.Entities;
using eShop.Shared.Enums;
using eShop.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eShop.Web.ViewModels
{
    public class PageViewModel
    {
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string PageURL { get; set; }
        public string PageImageURL { get; set; }

        public List<string> PageCanonicalURLs { get; set; }  //= new List<string>();

        public PageViewModel()
        {
            PageCanonicalURLs = new List<string>();
        }
    }

    public class CommentablePageViewModel : PageViewModel
    {
        public int EntityID { get; set; }
        public int RecordID { get; set; }

        public List<Comment> Comments { get; set; }
    }

    /// exemplu de unde am utilizat paginatia
    /// http://jasonwatmore.com/post/2015/10/30/aspnet-mvc-pagination-example-with-logic-like-google
    
    public class Pager
    {
        public Pager(
            int totalItems,
            int? page = 1,
            int recordSize = 10,
            int maxPages = 10)
        {
            // calculam numarul de pagini
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)recordSize);

            // ne asiguram ca pagina nu este out of range
            if (!page.HasValue || page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            int startPage, endPage;
            if (totalPages <= maxPages)
            {
                // numarul total de pagini mai mic decat max, afisam toate
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // numarul total de pagini mai mare decat max, calculam prima si ultima pagina
                var maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / (decimal)2);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / (decimal)2) - 1;
                if (page <= maxPagesBeforeCurrentPage)
                {
                    // pagina curenta este aproape la inceput
                    startPage = 1;
                    endPage = maxPages;
                }
                else if (page + maxPagesAfterCurrentPage >= totalPages)
                {
                    // pagina curenta este aproape la sfarsit
                    startPage = totalPages - maxPages + 1;
                    endPage = totalPages;
                }
                else
                {
                    // pagina curenta este aprox pe mijloc
                    startPage = page.Value - maxPagesBeforeCurrentPage;
                    endPage = page.Value + maxPagesAfterCurrentPage;
                }
            }

            // calculam indicii de start si de sfarsit pt items
            var startIndex = (page.Value - 1) * recordSize;
            var endIndex = Math.Min(startIndex + recordSize - 1, totalItems - 1);

            // se creaza o serie de pagini pentru redare in bucla
            var pages = Enumerable.Range(startPage, (endPage + 1) - startPage);

            // se actualizaeaza instanta obj cu toate propietatile din view
            TotalItems = totalItems;
            CurrentPage = page.Value;
            PageSize = recordSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Pages = pages;
        }

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }
        public IEnumerable<int> Pages { get; private set; }
    }
}