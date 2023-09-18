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
    public class FlowersController : Controller
    {
        // GET: Flowers
        private readonly DOTNETEntities8 entities = new DOTNETEntities8();
        public ActionResult Index()
        {
            List<Flower> FlowerList = entities.Flowers.ToList();
            return View(FlowerList);
        }

        public ActionResult UserIndex()
        {
            List<Flower> FlowerList = entities.Flowers.ToList();
            return View(FlowerList);
        }
        public ActionResult Addtocart(int? id)
        {
            Flower Veg = entities.Flowers.Find(id);
            TempData["Flowerid"] = Veg.fid;
            TempData["price"] = Veg.retailprice;
            TempData["name"] = Veg.name;
            TempData["image"] = Veg.image;
            TempData["userid"] = Session["UserID"];
            return View(Veg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addtocart(Flower veg, int quantity)
        {
            try
            {
                int? price = TempData["price"] as int?;
                string name = TempData["name"] as string;
                byte[] image = TempData["image"] as byte[];
                int? userId = TempData["userid"] as int?;
                int? fid = TempData["Flowerid"] as int?;

                if (ModelState.IsValid)
                {
                    var existingCartItem = entities.Carts.SingleOrDefault(c => c.userid == userId && c.Flowerid == fid);

                    if (existingCartItem != null)
                    {
                        existingCartItem.quantity += quantity;
                        existingCartItem.price = price * existingCartItem.quantity;
                    }
                    else
                    {                         

                        var newCartItem = new Cart
                        {
                            Flowerid = fid.Value,
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
        public ActionResult Create([Bind(Include = "fid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Flower flower, HttpPostedFileBase imageFile)
        {

            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        flower.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Flowers.Add(flower);
                entities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(flower);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            Flower flower = entities.Flowers.Find(id)
 ;
            return View(flower);
        }

        public ActionResult Delete(int? id)
        {
            Flower flower = entities.Flowers.Find(id)
;
            entities.Flowers.Remove(flower);
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

            Flower flower = entities.Flowers.Find(id);
            return View(flower);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Flower flower , HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imageFile.InputStream))
                    {
                        flower.image = reader.ReadBytes(imageFile.ContentLength);
                    }
                }
                entities.Entry(flower).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();
                return RedirectToAction("Index", "Flowers");
            }
            return View();
        }
    }
}