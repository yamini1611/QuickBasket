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
    public class PackedFoodController : Controller
    {
        // GET: PackedFood
        public DOTNETEntities8 entities = new DOTNETEntities8();
        public ActionResult Index()
        {
            List<packedfood> MeatList = entities.packedfoods.ToList();
            return View(MeatList);
        }
        public ActionResult UserIndex()
        {
            List<packedfood> MeatList = entities.packedfoods.ToList();
            return View(MeatList);
        }
        public ActionResult Addtocart(int? id)
        {
            packedfood Veg = entities.packedfoods.Find(id);
            TempData["pacid"] = Veg.pacid;
            TempData["price"] = Veg.retailprice;
            TempData["name"] = Veg.name;
            TempData["image"] = Veg.image;
            TempData["userid"] = Session["UserID"];
            return View(Veg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addtocart(packedfood veg, int quantity)
        {
            try
            {
                int? price = TempData["price"] as int?;
                string name = TempData["name"] as string;
                byte[] image = TempData["image"] as byte[];
                int? userId = TempData["userid"] as int?;
                int? pacid = TempData["pacid"] as int?;

                if (ModelState.IsValid)
                {
                    var existingCartItem = entities.Carts.SingleOrDefault(c => c.userid == userId && c.pacid == pacid);

                    if (existingCartItem != null)
                    {
                        existingCartItem.quantity += quantity;
                        existingCartItem.price = price * existingCartItem.quantity;
                    }
                    else
                    {
                        var newCartItem = new Cart
                        {
                           
                            pacid = pacid.Value,
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
        public ActionResult Create([Bind(Include = "pacid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] packedfood meat, HttpPostedFileBase imageFile)
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
                entities.packedfoods.Add(meat);
                entities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(meat);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            packedfood meat = entities.packedfoods.Find(id)
 ;
            return View(meat);
        }

        public ActionResult Delete(int? id)
        {
            packedfood meat = entities.packedfoods.Find(id)
 ;
            entities.packedfoods.Remove(meat);
            entities.SaveChanges();
            return RedirectToAction("Index" ,"PackedFood" );
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            packedfood meat = entities.packedfoods.Find(id);
            return View(meat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pacid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] packedfood meat, HttpPostedFileBase imageFile)
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