using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using La_Ñata.Models;

namespace La_Ñata.Controllers
{
    [Security]
    public class ProductsController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Products
        public ActionResult Index()
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
                List<Product> products = db.Product.Where(p => p.deleted_at.Equals(null)).ToList();
                return View(products);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado";
                TempData["Error"] = 2;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View(new Product());
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "description,stock,price,break_price")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product.created_at = DateTime.UtcNow.AddHours(-3);
                    db.Product.Add(product);
                    db.SaveChanges();

                    TempData["Message"] = "El producto se creó correctamente";

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Alguno de los campos ingresados no son válidos";
                    ViewBag.Error = 1;
                }

                return View(product);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido crear el producto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }


        // GET: Products/Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Product product = db.Product.Find(id);
                if (product == null)
                {
                    ViewBag.Message = "Ha ocurrido un error. No se ha encontrado el producto";
                    ViewBag.Error = 1;
                    product = new Product();
                }
                return View(product);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado.";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,description,stock,price,break_price")] Product product_edited)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Product.Find(product_edited.id);
                    product.description = product_edited.description;
                    product.stock = product_edited.stock;
                    product.price = product_edited.price;
                    product.break_price = product_edited.break_price;
                    db.SaveChanges();
                    TempData["Message"] = "El producto se guardó correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Alguno de los campos ingresados no son válidos";
                    ViewBag.Error = 1;
                }
                return View(product_edited);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido guardar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Product.Find(id);
                if (product != null)
                {
                    product.deleted_at = DateTime.UtcNow.AddHours(-3);
                    db.SaveChanges();
                    TempData["Message"] = "El producto fue eliminado";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Ha ocurrido un error. No se ha encontrado el producto";
                    TempData["Error"] = 1;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido eliminar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Filter products by name or internal code
        [HttpGet]
        public JsonResult FilterProductsByName(string name)
        {
            try
            {
                var products = db.Product
                        .Where(p =>
                        p.description.Contains(name) &&
                        p.deleted_at.Equals(null))
                        .Select(p => new
                        {
                            product_id = p.id,
                            p.description,
                            p.stock,
                            p.price,
                            p.break_price,
                        }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
