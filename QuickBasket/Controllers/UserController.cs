using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QuickBasket.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public DOTNETEntities8 entities = new DOTNETEntities8();

        // GET: User
        [Authorize(Roles = "Admin")]
        public ActionResult UsersIndex()
        {
            List<User> users = entities.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid ,Username ,password ,email ,Roleid ,phone ,modified_date")] User user)
        {

            if (ModelState.IsValid)
            {


                entities.Users.Add(user);
                entities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            User user = entities.Users.Find(id)
 ;
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            User user = entities.Users.Find(id)
;
            entities.Users.Remove(user);
            entities.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = entities.Users.Find(id);
            List<Role> role = entities.Roles.ToList();
            ViewBag.Roles = new SelectList(role, "Roleid", "RoleName");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid ,Username ,password ,email ,Roleid ,phone ,modified_date")] User user)
        {
            if (ModelState.IsValid)
            {
              
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();
                return RedirectToAction("UsersIndex", "User");
            }
            return View();
        }
    }
}