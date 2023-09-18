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

    public class VegetableController : Controller
    {
        public DOTNETEntities8 entities = new DOTNETEntities8();

        // GET: Vegetable
        public ActionResult Index()
        {
            List<Vegetable> vegetableList = entities.Vegetables.ToList();
            return View(vegetableList);
        }
        public ActionResult UserIndex()
        {
            List<Vegetable> vegetableList = entities.Vegetables.ToList();
            return View(vegetableList);
        }

      

        public ActionResult Addtocart(int? id)
        {
            Vegetable veg = entities.Vegetables.Find(id);
            TempData["vegid"] = veg.vegid;
            TempData["price"] = veg.retailprice;
            TempData["name"] = veg.name;
            TempData["image"] = veg.image;
            TempData["userid"] = Session["UserID"];
            return View(veg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addtocart(Vegetable veg, int quantity)
        {
            try
            {
                int? price = TempData["price"] as int?;
                string name = TempData["name"] as string;
                byte[] image = TempData["image"] as byte[];
                int? userId = TempData["userid"] as int?;
                int? vegid = TempData["vegid"] as int?;

                if (ModelState.IsValid)
                {
                    var existingCartItem = entities.Carts.SingleOrDefault(c => c.userid == userId && c.vegid == vegid);

                    if (existingCartItem != null)
                    {
                        existingCartItem.quantity += quantity;
                        existingCartItem.price = price * existingCartItem.quantity;
                    }
                    else
                    {
                        var newCartItem = new Cart
                        {
                           
                            vegid = vegid.Value,
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
        public ActionResult Create([Bind(Include ="vegid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Vegetable veg, HttpPostedFileBase imageFile)
        {
          
            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        veg.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Vegetables.Add(veg);
                entities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(veg);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
           Vegetable  Veg = entities.Vegetables.Find(id)
;
            return View(Veg);
        }

        public ActionResult Delete(int? id)
        {
            Vegetable veg = entities.Vegetables.Find(id)
;
            entities.Vegetables.Remove(veg);
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

            Vegetable veg = entities.Vegetables.Find(id);
            return View(veg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "vegid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Vegetable veg, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        veg.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Entry(veg).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();
                return RedirectToAction("Index" ,"Vegetable");
            }
            return View();
        }
    

}
}