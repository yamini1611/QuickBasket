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
using PagedList; 
namespace QuickBasket.Controllers
{
    [Authorize]
    public class VegetableController : Controller
    {
        public DOTNETEntities8 entities = new DOTNETEntities8();

        // GET: Vegetable
        [Authorize(Roles="Admin")]
        public ActionResult Index()
        {
            List<Vegetable> vegetableList = entities.Vegetables.ToList();
            return View(vegetableList);
        }


        public ActionResult UserIndex(string searchTerm, int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var vegetableList = entities.Vegetables.ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                vegetableList = vegetableList
                    .Where(v => v.name != null && v.name.ToLower().Contains(searchTerm))
                    .ToList();
            }

            // Create a PagedList from the filtered vegetableList
            var pagedVegetables = vegetableList.ToPagedList(pageNumber, pageSize);

            if (pagedVegetables.Count == 0 && !string.IsNullOrEmpty(searchTerm))
            {
                this.AddNotification("Searched product is not found.", NotificationType.ERROR);
            }

            return View(pagedVegetables);
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
                    this.AddNotification("Product Added To Cart", NotificationType.SUCCESS);

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

                else
                {
                    return RedirectToAction("Errror400");
                }
                entities.Vegetables.Add(veg);
                entities.SaveChanges();
                this.AddNotification("Product Added Successfully", NotificationType.SUCCESS);
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

            Vegetable veg = entities.Vegetables.Find(id);
            return View(veg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "vegid, name, originalcost, retailprice, stock, image, offer, category")] Vegetable veg, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
              Vegetable vegs = entities.Vegetables.Find(veg.vegid);

                if (vegs != null)
                {
                    vegs.name = veg.name;
                    vegs.originalcost = veg.originalcost;
                    vegs.retailprice = veg.retailprice;
                    vegs.stock = veg.stock;
                    vegs.offer = veg.offer;
                    vegs.category = veg.category;

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        using (var reader = new BinaryReader(imageFile.InputStream))
                        {
                            vegs.image = reader.ReadBytes(imageFile.ContentLength);
                        }
                    }

                    entities.Entry(vegs).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                    this.AddNotification("Product edited Successfully", NotificationType.SUCCESS);
                    return RedirectToAction("Index", "Vegetable");
                }

                else
                {
                    return RedirectToAction("Errror400");
                }
            }
            return View(veg);
        }


    }
}