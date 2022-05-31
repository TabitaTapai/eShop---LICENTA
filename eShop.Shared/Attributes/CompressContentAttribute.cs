using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eShop.Shared.Attributes
{
    /// <summary>
    /// Atribut care se poate adauga la metodele controlerului pentru fortare compresie GZIP
    /// </summary>
    public class CompressContentAttribute : ActionFilterAttribute
    {

        /// <summary>
        /// Suprascriere pt comprimare continut generat de o metoda de actiune
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GZipEncodePage();
        }

        /// <summary>
        /// determinare daca GZIP este suportat
        /// </summary>
        /// <returns></returns>
        public static bool IsGZipSupported()
        {
            string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(AcceptEncoding) &&
                    (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate")))
                return true;
            return false;
        }

        /// <summary>
        /// Setare pagina sau handler sa foloseasca GZIP prin Response.Filter
        /// Aceasta metoda trebuie apelata inainte de a genera orice rezultat
        /// </summary>
        public static void GZipEncodePage()
        {
            HttpResponse Response = HttpContext.Current.Response;

            if (IsGZipSupported())
            {
                string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

                if (AcceptEncoding.Contains("gzip"))
                {
                    Response.Filter = new System.IO.Compression.GZipStream(Response.Filter,
                                                System.IO.Compression.CompressionMode.Compress);
                    Response.Headers.Remove("Content-Encoding");
                    Response.AppendHeader("Content-Encoding", "gzip");
                }
                else
                {
                    Response.Filter = new System.IO.Compression.DeflateStream(Response.Filter,
                                                System.IO.Compression.CompressionMode.Compress);
                    Response.Headers.Remove("Content-Encoding");
                    Response.AppendHeader("Content-Encoding", "deflate");
                }


            }

            // Permite proxy serverelor sa retina separat versiunile codificate si necodificate
            Response.AppendHeader("Vary", "Content-Encoding");
        }
    }
}
