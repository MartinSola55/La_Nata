using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace La_Ñata.Models
{
    public class Security : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (!(HttpContext.Current.Session["User"] is User user))
                {
                    filterContext.Result = new RedirectResult("~/Login");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }
    public class Admin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (HttpContext.Current.Session["User"] is User user)
                {
                    if (user.Rol.id != 1)
                    {
                        filterContext.Result = new RedirectResult("~/Home");
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Login");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }
    public class Employee : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (HttpContext.Current.Session["User"] is User user)
                {
                    if (user.Rol.id != 2)
                    {
                        filterContext.Result = new RedirectResult("~/Home/AdminIndex");
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Login");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }
}