using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QuickBasket.Extensions;
using MailKit.Search;

namespace QuickBasket.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public DOTNETEntities8 entities = new DOTNETEntities8();

        // GET: User
        [Authorize(Roles = "Admin")]
        public ActionResult UsersIndex(string searchQuery, int? page)
        {
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            var usersQuery = entities.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower(); 
                usersQuery = usersQuery.Where(u =>
                    u.Username.ToLower().Contains(searchQuery) ||
                    u.email.ToLower().Contains(searchQuery) ||
                    u.Role.RoleName.ToLower().Contains(searchQuery)||
                    u.phone.ToLower().Contains(searchQuery));
            }

            var users = usersQuery.ToList();
            ViewBag.SearchQuery = searchQuery;

            var pagedList = users.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var totalItems = users.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var viewModel = new PagedViewModel<User>
            {
                Items = pagedList,
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid ,Username ,password ,email ,Roleid ,phone ,modified_date ,Avatar")] User user, HttpPostedFileBase imageFile)
        {

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        user.Avatar = reader.ReadBytes(imageFile.ContentLength);
                    }
                }

                else
                {
                    return RedirectToAction("Errror400");
                }
     
                entities.Users.Add(user);
                entities.SaveChanges();
                this.AddNotification("User Added Successfully", NotificationType.SUCCESS);

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
            if (id == null)
            {
                return RedirectToAction("Errror400");
            }

            User user = entities.Users.Find(id);
            Cart cart = entities.Carts.FirstOrDefault(c => c.userid== id);
            Order order = entities.Orders.FirstOrDefault(o => o.userid == id);

            if (user == null)
            {
                return RedirectToAction("Errror400");
            }

            if (cart != null)
            {
                this.AddNotification("Cannot delete a user who has products in the cart.", NotificationType.ERROR);
                return RedirectToAction("UsersIndex", "User");
            }

            if (order != null)
            {
                this.AddNotification("Cannot delete a user who has ordered products.", NotificationType.ERROR);
                return RedirectToAction("UsersIndex", "User");
            }

            entities.Users.Remove(user);
            this.AddNotification("User deleted Successfully", NotificationType.SUCCESS);

            entities.SaveChanges();

            return RedirectToAction("UsersIndex", "User");
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error400");
            }

            User user = entities.Users.Find(id);
            List<Role> role = entities.Roles.ToList();
            ViewBag.Roles = new SelectList(role, "Roleid", "RoleName");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid, Username, password, email, Roleid, phone, modified_date")] User user, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                User currentUser = entities.Users.Find(user.userid);

                if (currentUser != null)
                {
                    currentUser.Username = user.Username;
                    currentUser.email = user.email;
                    currentUser.phone = user.phone;
                    currentUser.Roleid = user.Roleid;

                    if (Avatar != null && Avatar.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(Avatar.InputStream))
                        {
                            currentUser.Avatar = reader.ReadBytes(Avatar.ContentLength);
                        }
                    }

                    entities.Entry(currentUser).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                    this.AddNotification("User profile edited Successfully", NotificationType.SUCCESS);

                    return RedirectToAction("UsersIndex", "User");
                }

                else
                {
                    return RedirectToAction("Error404");
                }
            }
            return View(user);
        }


        [HttpGet]
        public ActionResult UserProfile()
        {
            string username = HttpContext.User.Identity.Name;

            User user = entities.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Error404");
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error400");
            }

            User user = entities.Users.Find(id);
          

            return View(user);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User updatedUser, HttpPostedFileBase newAvatar)
        {
            if (ModelState.IsValid)
            {
                User currentUser = entities.Users.Find(updatedUser.userid);

                if (currentUser != null)
                {
                    currentUser.Username = updatedUser.Username;
                    currentUser.email = updatedUser.email;
                    currentUser.phone = updatedUser.phone;

                    if (newAvatar != null && newAvatar.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(newAvatar.InputStream))
                        {
                            currentUser.Avatar = reader.ReadBytes(newAvatar.ContentLength);
                        }
                    }

                    entities.SaveChanges();
                    this.AddNotification("Profile Edited Successfully", NotificationType.SUCCESS);

                    return RedirectToAction("Index","Home");
                }
            }

            return View(updatedUser);
        }



    }
}