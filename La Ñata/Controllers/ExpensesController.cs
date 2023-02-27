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
    public class ExpensesController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Expenses
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
                return View(new Expense());
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado";
                TempData["Error"] = 2;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Expenses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "date,description,price")] Expense expense)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Expense.Add(expense);
                    db.SaveChanges();
                    TempData["Message"] = "El gasto se creó correctamente";
                }
                else
                {
                    TempData["Message"] = "Alguno de los campos ingresados no son válidos";
                    TempData["Error"] = 1;
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido crear el gasto";
                TempData["Error"] = 2;
            }
            return RedirectToAction("Index");
        }

        // GET: Expenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expense.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,date,description,price")] Expense expense_edited)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Expense expense = db.Expense.Find(expense_edited.id);
                    expense.date = expense_edited.date;
                    expense.description = expense_edited.description;
                    expense.price = expense_edited.price;
                    db.SaveChanges();
                    TempData["Message"] = "El gasto se guardó correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Alguno de los campos ingresados no son válidos";
                    ViewBag.Error = 1;
                }
                return View(expense_edited);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido guardar el gasto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int expense_id)
        {
            try
            {
                Expense expense = db.Expense.Find(expense_id);
                if (expense != null)
                {
                    expense.deleted_at = DateTime.UtcNow.AddHours(-3);
                    db.SaveChanges();
                    TempData["Message"] = "El gasto fue eliminado";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Ha ocurrido un error. No se ha encontrado el gasto";
                    TempData["Error"] = 1;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido eliminar el gasto";
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

        // GET: Expenses by date
        [HttpGet]
        public JsonResult ExpensesByDate(string dates)
        {
            try
            {
                string[] dates_formated = dates.Trim().Split(',');
                DateTime date_from = Convert.ToDateTime(dates_formated[0]);
                DateTime date_to = Convert.ToDateTime(dates_formated[1]);

                var expenses = db.Expense
                    .Where(e => e.date >= date_from && e.date <= date_to && e.deleted_at.Equals(null))
                    .Select(e => new
                    {
                        e.id,
                        e.date,
                        e.description,
                        e.price,
                    })
                    .OrderByDescending(e => e.date).ThenByDescending(e => e.price)
                    .ToList();
                return Json(expenses, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: get one Expense
        [HttpGet]
        public JsonResult GetOne(int id)
        {
            try
            {
                var expense = db.Expense
                    .Where(e => e.id.Equals(id) && e.deleted_at.Equals(null))
                    .Select(e => new
                    {
                        e.date,
                        e.description,
                        e.price,
                    })
                    .First();
                return Json(expense, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
