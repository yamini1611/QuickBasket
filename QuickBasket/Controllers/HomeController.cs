using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuickBasket.Controllers
{
    
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public DOTNETEntities8 entities = new DOTNETEntities8();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User user)
        {
            Validate_User_Result roleUser = entities.Validate_User(user.Username, user.password).FirstOrDefault();
            string message;
            switch (roleUser.userid.Value)
            {
                case -1:
                    message = "Email / Password is incorrect.";
                    break;

                default:
                    Session["UserID"] = roleUser.userid;

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(30), false, roleUser.Roles, FormsAuthentication.FormsCookiePath);
                    string hash = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
                    if (ticket.IsPersistent)
                    {
                        cookie.Expires = ticket.Expiration;
                    }
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = message;
            return View(user);
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "userid,Username,Roleid,password,email,phone ")] User newUser)
        {
            if (ModelState.IsValid)
            {
                entities.Users.Add(newUser);
                entities.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}