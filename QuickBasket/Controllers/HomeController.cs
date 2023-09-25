using QuickBasket.Extensions;
using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
                    this.AddNotification("Email / Password is incorrect." ,notificationType:"Error");
                    message = "Email / Password is incorrect.";
                    break;

                default:
                    Session["UserID"] = roleUser.userid;
                    List<User> users = entities.Users.ToList();
                    foreach(User userdetail in users )
                    {
                        if(roleUser.userid == userdetail.userid)
                        {
                            Session["Avatar"] = userdetail.Avatar;

                        }
                    }
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
        public ActionResult Register(User newUser, string capturedImageData) 
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(capturedImageData))
                {
                    capturedImageData = capturedImageData.Replace("data:image/jpeg;base64,", "");

                    byte[] imageBytes = Convert.FromBase64String(capturedImageData);

                    newUser.Avatar = imageBytes;
                    entities.Users.Add(newUser);

                    entities.SaveChanges();
                    Debug.WriteLine("Redirecting to Login action");
                    return RedirectToAction("Login");

                }

            }

            return View();
        }

        private bool IsBase64String(string s)
        {
            try
            {
                byte[] data = Convert.FromBase64String(s);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
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