using PagedList;
using QuickBasket.Extensions;
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

        public ActionResult UserIndex(string searchTerm, int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var packedfoodList = entities.Fruits.ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                packedfoodList = packedfoodList
                    .Where(p => p.name != null && p.name.ToLower().Contains(searchTerm))
                    .ToList();
            }

            var pagedPackedFoods = packedfoodList.ToPagedList(pageNumber, pageSize);

            if (pagedPackedFoods.Count == 0 && !string.IsNullOrEmpty(searchTerm))
            {
                this.AddNotification("Searched product is not found.", NotificationType.ERROR);
            }

            return View(pagedPackedFoods); // Return a PagedList instead of IEnumerable
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
                        this.AddNotification("Product Added To Cart", NotificationType.SUCCESS);

                    }

                    entities.SaveChanges();
                    return RedirectToAction("CartView", "Cart");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding the product to the cart." + ex);

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

                else
                {
                    return RedirectToAction("Errror400");
                }
                entities.Fruits.Add(fruit);
                entities.SaveChanges();
                this.AddNotification("Product Added Successfully", NotificationType.SUCCESS);
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
            this.AddNotification("Product deleted Successfully", NotificationType.SUCCESS);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Errror400");
            }

            Fruit fruit = entities.Fruits.Find(id);
            return View(fruit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fid, name, originalcost, retailprice, stock, image, offer, category")] Fruit fruit, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                Fruit currentFruit = entities.Fruits.Find(fruit.fid);

                if (currentFruit != null)
                {
                    currentFruit.name = fruit.name;
                    currentFruit.originalcost = fruit.originalcost;
                    currentFruit.retailprice = fruit.retailprice;
                    currentFruit.stock = fruit.stock;
                    currentFruit.offer = fruit.offer;
                    currentFruit.category = fruit.category;

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(imageFile.InputStream))
                        {
                            currentFruit.image = reader.ReadBytes(imageFile.ContentLength);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Errror400");
                    }
                    entities.Entry(currentFruit).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                    this.AddNotification("Product Edited Successfully", NotificationType.SUCCESS);

                    return RedirectToAction("Index", "Fruit");
                }
            }
            return View(fruit);
        }

    }
}