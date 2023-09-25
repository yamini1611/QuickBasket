using PagedList;
using QuickBasket.Extensions;
using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;

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
        public ActionResult UserIndex(string searchTerm, int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var packedfoodList = entities.packedfoods.ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                packedfoodList = packedfoodList
                    .Where(p => p.name != null && p.name.ToLower().Contains(searchTerm))
                    .ToList();
            }

            // Create a PagedList from the filtered packedfoodList
            var pagedPackedFoods = packedfoodList.ToPagedList(pageNumber, pageSize);

            if (pagedPackedFoods.Count == 0 && !string.IsNullOrEmpty(searchTerm))
            {
                this.AddNotification("Searched product is not found.", NotificationType.ERROR);
            }

            return View(pagedPackedFoods); // Return a PagedList instead of IEnumerable
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
                        this.AddNotification("Product Added To Cart", NotificationType.SUCCESS);

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

                else
                {
                    return RedirectToAction("Errror400");
                }
                entities.packedfoods.Add(meat);
                entities.SaveChanges()
                this.AddNotification("Product Added Successfully", NotificationType.SUCCESS);

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
            this.AddNotification("Product Edited Successfully", NotificationType.SUCCESS);
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
        public ActionResult Edit([Bind(Include = "pacid, name, originalcost, retailprice, stock, image, offer, category")] packedfood packedfood, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                packedfood packedfoods = entities.packedfoods.Find(packedfood.pacid);

                if (packedfoods != null)
                {
                    packedfoods.name = packedfood.name;
                    packedfoods.originalcost = packedfood.originalcost;
                    packedfoods.retailprice = packedfood.retailprice;
                    packedfoods.stock = packedfood.stock;
                    packedfoods.offer = packedfood.offer;
                    packedfoods.category = packedfood.category;

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(imageFile.InputStream))
                        {
                            packedfoods.image = reader.ReadBytes(imageFile.ContentLength);
                        }
                    }

                    entities.Entry(packedfoods).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                    this.AddNotification("Product Edited Successfully", NotificationType.SUCCESS);

                    return RedirectToAction("Index", "Fruit");
                }

                else
                {
                    return RedirectToAction("Errror400");
                }
            }
            return View(packedfood);
        }

    }
}