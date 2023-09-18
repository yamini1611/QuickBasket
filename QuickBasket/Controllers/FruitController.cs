using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QuickBasket.Controllers
{
    public class FruitController : Controller
    {
        public DOTNETEntities8 entities = new DOTNETEntities8();

        // GET: Vegetable
        public ActionResult Index()
        {
            List<Fruit> FruitList = entities.Fruits.ToList();
            return View(FruitList);
        }

        public ActionResult UserIndex()
        {
            List<Fruit> FruitList = entities.Fruits.ToList();
            return View(FruitList);
        }
        [HttpGet]
        public ActionResult Addtocart(int? id)
        {
            Fruit fruit= entities.Fruits.Find(id);
            TempData["price"] = fruit.retailprice;
            TempData["name"] = fruit.name;
            TempData["image"] = fruit.image;
            TempData["userid"] = Session["UserID"];
            TempData["fid"] = fruit.fid;
            return View(fruit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addtocart(Fruit veg, int quantity)
        {
            try
            {
                int? price = TempData["price"] as int?;
                string name = TempData["name"] as string;
                byte[] image = TempData["image"] as byte[];
                int? userId = TempData["userid"] as int?;
                int? fid = TempData["fid"] as int?;

                // Debug statements to check the values
                System.Diagnostics.Debug.WriteLine($"userId: {userId}, fid: {fid}, quantity: {quantity}");

                if (ModelState.IsValid)
                {
                    var existingCartItem = entities.Carts.SingleOrDefault(c => c.userid == userId && c.fid == fid);

                    if (existingCartItem != null)
                    {
                        existingCartItem.quantity += quantity;
                        existingCartItem.price = price * existingCartItem.quantity;
                    }
                    else
                    {
                        var newCartItem = new Cart
                        {
                           
                            fid = fid.Value,
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

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Fruit fruit, HttpPostedFileBase imageFile)
        {

            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        fruit.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Fruits.Add(fruit);
                entities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fruit);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            Fruit fruit = entities.Fruits.Find(id)
 ;
            return View(fruit);
        }

        public ActionResult Delete(int? id)
        {
            Fruit fruit = entities.Fruits.Find(id)
;
            entities.Fruits.Remove(fruit);
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

            Fruit fruit = entities.Fruits.Find(id);
            return View(fruit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Fruit fruit, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        fruit.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Entry(fruit).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();
                return RedirectToAction("Index", "Fruit");
            }
            return View();
        }
    }
}