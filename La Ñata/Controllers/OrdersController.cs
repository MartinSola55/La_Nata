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
using System.Web.Script.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace La_Ñata.Controllers
{
    [Security]
    public class OrdersController : Controller
    {
        private EFModel db = new EFModel();
        private string ToTitleCase(string title)
        {
            title = Regex.Replace(title, @"[^a-zA-Z\u00C0-\u017F\s']", string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
        private string ToUpperFirst(string title)
        {
            title = Regex.Replace(title, @"[^0-9a-zA-Z\u00C0-\u017F\s']", string.Empty);
            string qsy = title.ToUpper()[0] + title.ToLower().Substring(1);
            return title.ToUpper()[0] + title.ToLower().Substring(1);
        }
        private string ToNumber(string numero)
        {
            numero = Regex.Replace(numero, @"[^0-9]", string.Empty);
            return numero;
        }

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
                List<Order> orders = db.Order.Where(o => DbFunctions.TruncateTime(o.date).ToString().Contains(date) && o.deleted_at.Equals(null)).ToList();
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
                    TempData["Message"] = "Debes seleccionar un pedido para ver los detalles";
                    TempData["Error"] = 1;
                    return RedirectToAction("Index");
                }
                Order order = db.Order.Where(o => o.id.Equals(id.Value) && o.deleted_at.Equals(null)).FirstOrDefault();
                if (order == null)
                {
                    TempData["Message"] = "El pedido que deseas ver no existe";
                    TempData["Error"] = 1;
                    return RedirectToAction("Index");
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
                return View(new Order());
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Debes seleccionar un pedido para editar";
                TempData["Error"] = 1;
                return RedirectToAction("Index");
            }
            Order order = db.Order.Where(o => o.id.Equals(id.Value) && o.deleted_at.Equals(null)).FirstOrDefault();
            if (order == null)
            {
                TempData["Message"] = "El pedido que deseas editar no existe";
                TempData["Error"] = 1;
                return RedirectToAction("Index");
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
            List<Order> orders = db.Order.Where(o => DbFunctions.TruncateTime(o.date).ToString().Contains(date_formatted) && o.id != order.id && o.deleted_at.Equals(null)).ToList();

            int prod_anterior = -1;
            foreach (ProductOrder item in order.ProductOrder)
            {
                // Restar stock del mismo producto sin volver a revisar los pedidos del mismo día
                if (prod_anterior == item.id_product)
                {
                    item.Product.stock -= item.quantity.Value;
                }
                // Si no se repite el producto en el pedido
                else
                {
                    prod_anterior = item.id_product;
                    int quant_prod_orders = 0;
                    foreach (Order ord in orders)
                    {
                        foreach (ProductOrder prod_order in ord.ProductOrder)
                        {
                            // Si el producto se repite en otro pedido
                            if (prod_order.id_product == item.id_product)
                            {
                                quant_prod_orders += prod_order.quantity.Value;
                            }
                        }
                    }
                    // Resta la cantidad en otros pedidos
                    item.Product.stock -= quant_prod_orders;

                    //Resta la cantidad del pedido actual
                    item.Product.stock -= item.quantity.Value;
                }
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Order order = db.Order.Where(o => o.id.Equals(id) && o.deleted_at.Equals(null)).First();
                order.deleted_at = DateTime.UtcNow.AddHours(-3);
                db.SaveChanges();
                TempData["Message"] = "El pedido se ha eliminado correctamente";
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error. El pedido no pudo ser eliminado";
                TempData["Error"] = 2;
            }
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
                    .Where(o => DbFunctions.TruncateTime(o.date).ToString().Contains(date_formatted) && o.deleted_at.Equals(null))
                    .Select(o => new
                    {
                        o.id,
                        o.client_name,
                        o.phone,
                        o.event_adress,
                        o.date,
                        o.shipment_price,
                        o.observation,
                    })
                    .OrderByDescending(o => o.date)
                    .ToList();
                return Json(orders, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Set Order data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetOrderData(Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.client_name = ToTitleCase(order.client_name);
                    order.event_adress = ToUpperFirst(order.event_adress);
                    order.phone = order.phone == null ? null : ToNumber(order.phone);
                    Session["Order"] = order;
                    return RedirectToAction("AddProducts");
                }
                TempData["Message"] = "Alguno de los campos ingresados no es válido";
                TempData["Message"] = 1;
                return RedirectToAction("Create", order);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error al intentar crear el pedido";
                TempData["Message"] = 2;
                return RedirectToAction("Index");
            }
        }

        // GET: View to see products in the order
        [HttpGet]
        public ActionResult AddProducts()
        {
            if (Session["Order"] != null)
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
                Order order = Session["Order"] as Order;
                return View(new ProductOrder { Order = order });
            }
            TempData["Message"] = "Debes volver a ingresar los datos del pedido";
            TempData["Error"] = 1;
            return RedirectToAction("Create");
        }


        // GET: Add products to order
        [HttpGet]
        public ActionResult AddToOrder(int? id_product, int? quantity, int stock)
        {
            try
            {
                Order order = Session["Order"] as Order;
                List<ProductOrder> products = new List<ProductOrder>();
                if (id_product != null && quantity != null)
                {
                    if (Session["Products"] != null)
                    {
                        products = Session["Products"] as List<ProductOrder>;
                    }
                    if (quantity.Value > 0)
                    {
                        ProductOrder prod = new ProductOrder();
                        prod.Product = db.Product
                            .Where(p => p.id.Equals(id_product.Value))
                            .First();

                        if (products.Count == 0)
                        {
                            prod.quantity = quantity.Value;
                            prod.unit_price = prod.Product.price;
                            products.Add(prod);
                            TempData["Message"] = "El producto se agregó correctamente";
                            Session["Products"] = products;
                            return RedirectToAction("AddProducts");
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
                                    if (products[index].quantity + quantity.Value <= stock)
                                    {
                                        products[index].quantity += quantity.Value;
                                        TempData["Message"] = "El producto se sumó correctamente";
                                        Session["Products"] = products;
                                        return RedirectToAction("AddProducts");
                                    }
                                    TempData["Message"] = "No cuentas con suficiente stock";
                                    TempData["Error"] = 1;
                                    return RedirectToAction("AddProducts");
                                }
                                // Add new product
                                else if (index == products.Count - 1)
                                {
                                    prod.quantity = quantity.Value;
                                    prod.unit_price = prod.Product.price;
                                    products.Add(prod);
                                    TempData["Message"] = "El producto se agregó correctamente";
                                    Session["Products"] = products;
                                    return RedirectToAction("AddProducts");
                                }
                            }
                        }
                    }
                    TempData["Message"] = "Por favor, ingresa una cantidad mayor a cero";
                    TempData["Error"] = 1;
                    return RedirectToAction("AddProducts");
                }
                TempData["Message"] = "No has ingresado correctamente el producto o la cantidad";
                TempData["Error"] = 1;
                return RedirectToAction("AddProducts");
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. Por favor, vuelve a agregar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("AddProducts");
            }
        }

        // GET: Remove products from order
        [HttpGet]
        public ActionResult RemoveProduct(int? id_product)
        {
            try
            {
                if (id_product != null && Session["Products"] != null)
                {
                    List<ProductOrder> products = Session["Products"] as List<ProductOrder>;
                    ProductOrder prod_s = products.Where(p => p.Product.id.Equals(id_product.Value)).FirstOrDefault();
                    products.Remove(prod_s);
                    if (products.Count > 0)
                    {
                        Session["Products"] = products;
                    }
                    else
                    {
                        Session["Products"] = null;
                    }
                    TempData["Message"] = "El producto se eliminó correctamente";
                    return RedirectToAction("AddProducts");
                }
                TempData["Message"] = "Ocurrió un problema con el producto seleccionado";
                TempData["Error"] = 1;
                return RedirectToAction("AddProducts");
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. Por favor, vuelve a agregar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("AddProducts");
            }
        }

        // POST: Create order
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Session["Products"] != null)
                    {
                        List<ProductOrder> products = Session["Products"] as List<ProductOrder>;

                        order.created_at = DateTime.UtcNow.AddHours(-3);
                        order.observation = order.observation == null ? null : ToUpperFirst(order.observation);

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
                        Session["Order"] = null;
                        TempData["Message"] = "El pedido se ha creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    TempData["Message"] = "No hay ningun producto cargado en el pedido";
                    TempData["Error"] = 1;
                    return View(order);
                }
                TempData["Message"] = "Por favor, ingrese una observación válida";
                TempData["Error"] = 1;
                return RedirectToAction("AddProducts");
            }
            catch (Exception)
            {
                Session["Products"] = null;
                Session["Order"] = null;
                TempData["Message"] = "Ha ocurrido un error. El pedido no pudo ser creado";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Add products to existing order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToExistingOrder(int? id_product, int? quantity, int real_stock, int id_order, double? old_price)
        {
            try
            {
                Order order = db.Order.Where(o => o.id.Equals(id_order) && o.deleted_at.Equals(null)).First();
                if (id_product != null && quantity != null && order != null)
                {
                    List<ProductOrder> products = db.ProductOrder.Where(po => po.id_order.Equals(order.id)).ToList();
                    if (quantity.Value > 0)
                    {
                        ProductOrder prod = new ProductOrder
                        {
                            Product = db.Product
                            .Where(p => p.id.Equals(id_product.Value))
                            .First()
                        };

                        if (products.Count == 0)
                        {
                            prod.quantity = quantity.Value;
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
                                        if (quantity.Value <= real_stock)
                                        {
                                            // Validate to be the same price
                                            if (old_price.Value == products[index].unit_price)
                                            {
                                                products[index].quantity += quantity.Value;
                                                db.SaveChanges();
                                                break;
                                            }
                                        } else
                                        {
                                            TempData["Message"] = "El producto no cuenta con suficiente stock";
                                            TempData["Error"] = 1;
                                            return RedirectToAction("Edit", new { id = id_order });
                                        }
                                    }
                                    else
                                    {
                                        prod.quantity = quantity.Value;
                                        prod.unit_price = prod.Product.price;
                                        prod.id_product = prod.Product.id;
                                        prod.id_order = id_order;
                                        db.ProductOrder.Add(prod);
                                        db.SaveChanges();
                                        break;
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
                                    prod.quantity = quantity.Value;
                                    prod.id_product = prod.Product.id;
                                    prod.id_order = id_order;
                                    db.ProductOrder.Add(prod);
                                    db.SaveChanges();
                                }
                            }
                        }
                        TempData["Message"] = "El producto se agregó correctamente al pedido";
                        return RedirectToAction("Edit", new { id = id_order });
                    }
                    TempData["Message"] = "Seleccione una cantidad mayor a 0";
                    TempData["Error"] = 1;
                    return RedirectToAction("Edit", new { id = id_order });
                }
                TempData["Message"] = "Ha ocurrido un error. Intente nuevamente";
                TempData["Error"] = 1;
                return RedirectToAction("Edit", new { id = id_order });
            }
            catch (DbUpdateException)
            {
                TempData["Message"] = "Ya existe un producto con el mismo precio en el pedido";
                TempData["Error"] = 1;
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error. El producto no se ha podido agregar correctamente al pedido";
                TempData["Error"] = 2;
            }
            return RedirectToAction("Edit", new { id = id_order });
        }

        // POST: Remove products from existing order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromExistingOrder(int? id_product, int id_order, double? price, int? quantity)
        {
            try
            {
                ProductOrder product_order = db.ProductOrder
                    .Where(po => po.id_product.Equals(id_product.Value) && po.id_order.Equals(id_order) && po.unit_price.Equals(price.Value))
                    .First();
                if (product_order != null)
                {
                    if (quantity.Value > 0)
                    {
                        // Validate not to have negative stock
                        if (quantity.Value > product_order.quantity)
                        {
                            TempData["Message"] = "Debes quitar una cantidad menor o igual a la reservada";
                            TempData["Error"] = 1;
                            return RedirectToAction("Edit", new { id = id_order });
                        }
                        // Remove product from order
                        else if (quantity.Value == product_order.quantity)
                        {
                            db.ProductOrder.Remove(product_order);
                            db.SaveChanges();
                            TempData["Message"] = "El producto se eliminó correctamente del pedido ";
                            return RedirectToAction("Edit", new { id = id_order });
                        }
                        // Substract quantity
                        else
                        {
                            product_order.quantity -= quantity.Value;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Seleccione una cantidad mayor a cero";
                        TempData["Error"] = 1;
                        return RedirectToAction("Edit", new { id = id_order });
                    }
                }
                else
                {
                    TempData["Message"] = "Ha ocurrido un error y no se ha encontrado el producto. Intente nuevamente";
                    TempData["Error"] = 1;
                    return RedirectToAction("Edit", new { id = id_order });
                }
                TempData["Message"] = "El producto se restó correctamente del pedido";
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error. El producto no se ha podido restar correctamente del pedido";
                TempData["Error"] = 2;
            }
            return RedirectToAction("Edit", new { id = id_order });
        }

        // GET: Order report with price
        [HttpGet]
        public ActionResult ReportWithPrice(Order order)
        {
            return View(order);
        }

        // GET: Order report without price
        [HttpGet]
        public ActionResult ReportWithoutPrice(Order order)
        {            
            return View(order);
        }

        // GET: Print report
        [HttpGet]
        public ActionResult Print(int id_order, bool price)
        {
            try
            {
                Order order = db.Order.Find(id_order);
                if (price)
                {
                    return View("ReportWithPrice", order);
                }
                else
                {
                    return View("ReportWithoutPrice", order);
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido generar el detalle";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }
    }
}
