using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using La_Ñata.Models;
using System.Transactions;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

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

            if (TempData.Count == 1)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            else if (TempData.Count == 2)
            {
                ViewBag.Message = TempData["Message"].ToString();
                ViewBag.Error = TempData["Error"];
            }

            string date_formatted = Convert.ToDateTime(order.date.Date).ToString("yyyy-MM-dd");
            List<Order> orders = db.Order.Where(o => DbFunctions.TruncateTime(o.date).ToString().Contains(date_formatted) && o.id != order.id).ToList();

            // REVISAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAR
            foreach (ProductOrder item in order.ProductOrder)
            {
                int quant_prod_orders = 0;
                foreach (Order ord in orders)
                {
                    foreach (ProductOrder prod_order in ord.ProductOrder)
                    {
                        if (prod_order.id_product == item.id_product)
                        {
                            quant_prod_orders += prod_order.quantity.Value;
                        }
                    }
                }
                item.Product.stock -= quant_prod_orders;
                item.Product.stock -= item.quantity.Value;
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


        // GET: Add products to order
        [HttpGet]
        public void AddToOrder(int? id_prod, int? quant, int real_stock)
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
                            // Check for duplicate product
                            int count = products.Count;
                            for (int index = 0; index < count; index++)
                            {
                                // Add quantity to same product
                                if (products[index].Product.id == prod.Product.id)
                                {
                                    // Validate not to exceed stock
                                    if (products[index].quantity + quant.Value <= real_stock)
                                    {
                                        products[index].quantity += quant.Value;
                                        break;
                                    }
                                }
                                // Add new product
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
            }
            catch (Exception)
            {
                throw;
            }
            return;
        }

        // GET: Remove products from order
        [HttpGet]
        public void RemoveProduct(int? id_prod)
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
            return;
        }

        // POST: Create order
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(Order order)
        {
            try
            {
                if (Session["Products"] != null)
                {
                    List<ProductOrder> products = Session["Products"] as List<ProductOrder>;

                    order.created_at = DateTime.UtcNow.AddHours(-3);

                    using (var transaccion = new TransactionScope())
                    {
                        db.Order.Add(order);
                        db.SaveChanges();
                        foreach (ProductOrder item in products)
                        {
                            ProductOrder prod_order = new ProductOrder
                            {
                                id_product = item.Product.id,
                                id_order = order.id,
                                quantity = item.quantity,
                                unit_price = item.unit_price,
                            };

                            db.ProductOrder.Add(prod_order);
                        }
                        db.SaveChanges();
                        transaccion.Complete();
                    }
                    Session["Products"] = null;
                    TempData["Message"] = "La orden se ha creado exitosamente";
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error. La orden no pudo ser creada";
                TempData["Error"] = "2";
            }
            return RedirectToAction("Index");
        }

        // GET: Add products to existing order
        [HttpGet]
        public ActionResult AddToExistingOrder(int? id_prod, int? quant, int real_stock, int id_order, double? old_price)
        {
            try
            {
                Order order = db.Order.Find(id_order);
                if (id_prod != null && quant != null && order != null)
                {
                    List<ProductOrder> products = db.ProductOrder.Where(po => po.id_order.Equals(order.id)).ToList();
                    if (quant.Value > 0)
                    {
                        ProductOrder prod = new ProductOrder
                        {
                            Product = db.Product
                            .Where(p => p.id.Equals(id_prod.Value))
                            .First()
                        };

                        if (products.Count == 0)
                        {
                            prod.quantity = quant.Value;
                            prod.unit_price = prod.Product.price;
                            prod.id_product = prod.Product.id;
                            prod.id_order = id_order;
                            db.ProductOrder.Add(prod);
                            db.SaveChanges();
                        }
                        else
                        {
                            // Check for duplicate product
                            int count = products.Count;
                            for (int index = 0; index < count; index++)
                            {
                                // Add quantity to same product
                                if (products[index].Product.id == prod.Product.id)
                                {
                                    // Validate to have the same price
                                    if (old_price != null)
                                    {
                                        // Validate not to exceed stock
                                        if (products[index].quantity + quant.Value <= real_stock)
                                        {
                                            products[index].quantity += quant.Value;
                                            break;
                                        }
                                    } else
                                    {
                                        prod.quantity = quant.Value;
                                        prod.unit_price = prod.Product.price;
                                        prod.id_product = prod.Product.id;
                                        prod.id_order = id_order;
                                        db.ProductOrder.Add(prod);
                                        db.SaveChanges();
                                    }
                                }
                                // Add new product
                                else if (index == products.Count - 1)
                                {
                                    if (old_price == null)
                                    {
                                        prod.unit_price = prod.Product.price;
                                    } else
                                    {
                                        prod.unit_price = old_price.Value;
                                    }
                                    prod.quantity = quant.Value;
                                    prod.id_product = prod.Product.id;
                                    prod.id_order = id_order;
                                    db.ProductOrder.Add(prod);
                                    db.SaveChanges();
                                }
                            }
                        }
                        TempData["Message"] = "El producto se agregó correctamente a la order";
                    }
                }
            }
            catch (DbUpdateException)
            {
                TempData["Message"] = "Ya existe un producto con el mismo precio en la orden";
                TempData["Error"] = 1;
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error. El producto no se ha podido agregar correctamente a la order";
                TempData["Error"] = 2;
            }
            return View("Edit", new { id = id_order });
        }
    }
}
