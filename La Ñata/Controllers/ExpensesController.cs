using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using La_Ñata.Models;
using Rotativa;

namespace La_Ñata.Controllers
{
    public class ExpensesController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Expenses
        [Security]
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
        [Security]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Security]
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
        [Security]
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
        [Security]
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
        [Security]
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

        [Security]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Expenses by date
        [Security]
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
        [Security]
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

        
        public ActionResult Report(string json)
        {
            try
            {
                ReportViewModel model = new JavaScriptSerializer().Deserialize<ReportViewModel>(json);
                List<Order> orders = new List<Order>();
                foreach (int order_id in model.IDOrders)
                {
                    Order order = db.Order.Find(order_id);
                    orders.Add(order);
                }
                model.Orders = orders;
                return View(model);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido generar el reporte";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // GET: Print report
        [Security]
        [HttpGet]
        public ActionResult Print(string dates_range)
        {
            try
            {
                ReportViewModel model = new ReportViewModel();

                string[] dates_formated = dates_range.Trim().Split(',');
                DateTime date_from = Convert.ToDateTime(dates_formated[0]);
                DateTime date_to = Convert.ToDateTime(dates_formated[1]);

                List<Expense> expenses = db.Expense
                    .Where(e => e.date >= date_from && e.date <= date_to && e.deleted_at.Equals(null))
                    .OrderByDescending(e => e.date).ThenByDescending(e => e.price)
                    .ToList();

                List<int> orders = db.Order
                    .Where(o => o.date >= date_from && o.date <= date_to && o.deleted_at.Equals(null))
                    .Select(o => o.id)
                    .ToList();

                model.Expenses = expenses;
                model.IDOrders = orders;
                model.DateFrom = date_from;
                model.DateTo = date_to;


                var json = new JavaScriptSerializer().Serialize(model);

                var report = new ActionAsPdf("Report", new { json })
                {
                    FileName = "Reporte balance " + model.DateFrom.ToString("dd-MM-yyyy") + " al " + model.DateTo.ToString("dd-MM-yyyy") + ".pdf",
                };
                return report;
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido generar el reporte";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }
        public class ReportViewModel
        {
            public List<Expense> Expenses { get; set; }
            public List<int> IDOrders { get; set; }
            public List<Order> Orders { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }
    }
}
