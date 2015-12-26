using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo02.Controllers
{
    public class LuceneNetController : Controller
    {
        // GET: LuceneNet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Demo1()
        {
            return View();
        }

        public ActionResult CreateIndex()
        {
            string indexPath = HttpContext.Server.MapPath("~/IndexData");

            //FSDirectory

            return Content(indexPath);

        }
    }
}