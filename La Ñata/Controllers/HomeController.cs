using La_Ñata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace La_Ñata.Controllers
{
    [Security]
    public class HomeController : Controller
    {
        private readonly EFModel db = new EFModel();
        // GET: Home
        public ActionResult Index()
        {
            try
            {
                string payment = null;
                if (DateTime.UtcNow.AddHours(-3).Day == 9)
                {
                    API_Obj currency = Import();
                    if (currency.result == "success")
                    {
                        payment = ((currency.conversion_rates.ARS * 15) * 2).ToString("N0", new System.Globalization.CultureInfo("is-IS"));
                    }
                }

                int totalProducts = db.Product.Where(p => p.deleted_at.Equals(null)).Count();
                ViewBag.TotalProducts = totalProducts;
                ViewBag.AmountToPay = payment;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Ha ocurrido un error insesperado. Por favor, vuelve a cargar la página";
                ViewBag.Error = 2;
                return View();
            }
        }
        public static API_Obj Import()
        {
            try
            {
                string URLString = "https://v6.exchangerate-api.com/v6/591370ea12622c46e1d175f0/latest/USD";
                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(URLString);
                    API_Obj Test = JsonConvert.DeserializeObject<API_Obj>(json);
                    return Test;
                }
            }
            catch (Exception)
            {
                return new API_Obj();
            }
        }

        public class API_Obj
        {
            public string result { get; set; }
            public string documentation { get; set; }
            public string terms_of_use { get; set; }
            public string time_last_update_unix { get; set; }
            public string time_last_update_utc { get; set; }
            public string time_next_update_unix { get; set; }
            public string time_next_update_utc { get; set; }
            public string base_code { get; set; }
            public ConversionRate conversion_rates { get; set; }
        }

        public class ConversionRate
        {
            public double ARS { get; set; }
        }
    }
}