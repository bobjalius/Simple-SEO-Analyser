using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BizComponent;

namespace SimpleSEOAnalyser.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ReturnResult res)
        {
            SimpleSEOComponent sc = new SimpleSEOComponent();

            ReturnResult result = sc.AnalyzeFromURL(res.TextSearch, res.FilterStopWords, res.ShowWordOccurence, res.ShowMetaOccurence, res.ShowLinks);

            return View("Index",result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

}