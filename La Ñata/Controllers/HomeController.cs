using La_Ñata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace La_Ñata.Controllers
{
    [Security]
    public class HomeController : Controller
    {
        private EFModel db = new EFModel();
        // GET: Home
        public ActionResult Index()
        {
            int totalProducts = db.Product.Where(p => p.deleted_at.Equals(null)).Count();
            ViewBag.TotalProducts = totalProducts;
            return View();
        }
    }
}