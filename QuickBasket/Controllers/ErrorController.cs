using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuickBasket.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Error400()
        {
            return View();
        }

        public ActionResult Error201()
        {
            return View();
        }

        public ActionResult Error500() 
        {
            return View();
        }

        public ActionResult Error403()
        {
            return View();
        }
    }
}