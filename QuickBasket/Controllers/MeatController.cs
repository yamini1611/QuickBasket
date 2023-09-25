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
        public ActionResult UserIndex(string searchTerm, int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var packedfoodList = entities.Meats.ToList();

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
                else
                {
                    return RedirectToAction("Errror400");
                }
                entities.Meats.Add(meat);
                entities.SaveChanges();
                this.AddNotification("Product Added Successfully", NotificationType.SUCCESS);
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
            this.AddNotification("Product removed Successfully", NotificationType.SUCCESS);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Errror400");
            }

            Meat meat = entities.Meats.Find(id);
            return View(meat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mid, name, originalcost, retailprice, stock, image, offer, category")] Meat meat, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                Meat currentmeat = entities.Meats.Find(meat.mid);

                if (currentmeat != null)
                {
                    currentmeat.name = meat.name;
                    currentmeat.originalcost = meat.originalcost;
                    currentmeat.retailprice = meat.retailprice;
                    currentmeat.stock = meat.stock;
                    currentmeat.offer = meat.offer;
                    currentmeat.category = meat.category;

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(imageFile.InputStream))
                        {
                            currentmeat.image = reader.ReadBytes(imageFile.ContentLength);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Errror400");
                    }
                    entities.Entry(currentmeat).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                    this.AddNotification("Product Edited Successfully", NotificationType.SUCCESS);

                    return RedirectToAction("Index", "Meat");
                }
            }
            return View(meat);
        }

    }
}