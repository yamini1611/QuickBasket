using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace QuickBasket.Controllers
{
    public class MeatController : Controller
    {
        // GET: Meat
        public DOTNETEntities8 entities = new DOTNETEntities8();
        public ActionResult Index()
        {
            List<Meat> MeatList = entities.Meats.ToList();
            return View(MeatList);
        }
        public ActionResult UserIndex()
        {
            List<Meat> MeatList = entities.Meats.ToList();
            return View(MeatList);
        }

        public ActionResult Addtocart(int? id)
        {
            Meat Veg = entities.Meats.Find(id);
            TempData["mid"] = Veg.mid;
            TempData["price"] = Veg.retailprice;
            TempData["name"] = Veg.name;
            TempData["image"] = Veg.image;
            TempData["userid"] = Session["UserID"];
            return View(Veg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addtocart(Meat veg, int quantity)
        {
            try
            {
                int? price = TempData["price"] as int?;
                string name = TempData["name"] as string;
                byte[] image = TempData["image"] as byte[];
                int? userId = TempData["userid"] as int?;
                int? mid = TempData["mid"] as int?;

                System.Diagnostics.Debug.WriteLine($"userId: {userId}, mid: {mid}, quantity: {quantity}");

                if (ModelState.IsValid)
                {
                    var existingCartItem = entities.Carts.SingleOrDefault(c => c.userid == userId && c.mid == mid);

                    if (existingCartItem != null)
                    {
                        existingCartItem.quantity += quantity;
                        existingCartItem.price = price * existingCartItem.quantity;
                    }
                    else
                    {
                        var newCartItem = new Cart
                        {
                            mid = mid.Value,
                            userid = userId,
                            quantity = quantity,
                            image = image,
                            name = name,
                            price = price * quantity
                        };

                        entities.Carts.Add(newCartItem);
                    }

                    entities.SaveChanges();
                    return RedirectToAction("CartView", "Cart");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding the product to the cart." + ex);

                // Debug statement to log the exception
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }

            return View(veg);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Meat meat, HttpPostedFileBase imageFile)
        {

            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        meat.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Meats.Add(meat);
                entities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(meat);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            Meat meat = entities.Meats.Find(id)
 ;
            return View(meat);
        }

        public ActionResult Delete(int? id)
        {
           Meat meat = entities.Meats.Find(id)
;
            entities.Meats.Remove(meat);
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

            Meat meat = entities.Meats.Find(id);
            return View(meat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Meat meat, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        meat.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Entry(meat).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();
                return RedirectToAction("Index", "Meat");
            }
            return View();
        }
    
}
}