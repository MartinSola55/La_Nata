using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using La_Ñata.Models;

namespace La_Ñata.Controllers
{
    [Security]
    public class OrdersController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Orders
        public ActionResult Index(string date)
        {
            try
            {
                if (TempData.Count == 1)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }
                else if (TempData.Count == 2)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                    ViewBag.Error = TempData["Error"];
                }
                date = date != null ? date : DateTime.UtcNow.AddHours(-3).ToString("yyyy-MM-dd");
                List<Order> orders = db.Order.Where(o => DbFunctions.TruncateTime(o.date).ToString().Contains(date)).ToList();
                return View(orders);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado";
                TempData["Error"] = 2;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Order order = db.Order.Find(id);
                if (order == null)
                {
                    return HttpNotFound();
                }
                if (TempData.Count == 1)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }
                else if (TempData.Count == 2)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                    ViewBag.Error = TempData["Error"];
                }
                return View(order);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado";
                TempData["Error"] = 2;
                return RedirectToAction("Index", new { date = DateTime.UtcNow.AddHours(-3) });
            }
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View(new Order());
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,client_name,phone,event_adress,date,shipment_price,observation,created_at,deleted_at")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Order.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,client_name,phone,event_adress,date,shipment_price,observation,created_at,deleted_at")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Orders by date
        public JsonResult OrdersByDate(string date)
        {
            try
            {
                string date_formatted = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                var orders = db.Order
                    .Where(o => DbFunctions.TruncateTime(o.date).ToString().Contains(date_formatted))
                    .Select(o => new
                    {
                        o.id,
                        o.client_name,
                        o.phone,
                        o.event_adress,
                        o.date,
                        o.shipment_price,
                        o.observation,
                    }).ToList();
                return Json(orders, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Add products to order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProducts(Order order)
        {
            if (ModelState.IsValid)
            {
                return View(new ProductOrder { Order = order });
            }
            return RedirectToAction("Create");
        }


        // GET: Add products to cart
        [HttpGet]
        public void AddToOrder(int? id_prod, int? quant)
        {
            try
            {
                List<ProductOrder> products = new List<ProductOrder>();
                if (id_prod != null && quant != null)
                {
                    if (Session["Products"] != null)
                    {
                        products = Session["Products"] as List<ProductOrder>;
                    }
                    if (quant.Value > 0)
                    {
                        ProductOrder prod = new ProductOrder();
                        prod.Product = db.Product
                            .Where(p => p.id.Equals(id_prod.Value))
                            .First();
                        if (products.Count == 0)
                        {
                            prod.quantity = quant.Value;
                            prod.unit_price = prod.Product.price;
                            products.Add(prod);
                        }
                        else
                        {
                            int count = products.Count;
                            for (int index = 0; index < count; index++)
                            {
                                if (products[index].Product.id == prod.Product.id)
                                {
                                    products[index].quantity += quant.Value;
                                    break;
                                }
                                else if (index == products.Count - 1)
                                {
                                    prod.quantity = quant.Value;
                                    prod.unit_price = prod.Product.price;
                                    products.Add(prod);
                                }
                            }
                        }
                        Session["Products"] = products;
                    }
                }
                return;
            }
            catch (Exception)
            {
                Session["Products"] = null;
                return;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveProductSell(int? id_prod)
        {
            try
            {
                if (id_prod != null && Session["Products"] != null)
                {
                    List<ProductOrder> products = Session["Products"] as List<ProductOrder>;
                    ProductOrder prod_s = products.Where(p => p.Product.id.Equals(id_prod.Value)).FirstOrDefault();
                    products.Remove(prod_s);
                    if (products.Count > 0)
                    {
                        Session["Products"] = products;
                    }
                    else
                    {
                        Session["Products"] = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("AddProducts");
        }
    }
}
