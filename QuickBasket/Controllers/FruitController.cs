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
        public DOTNETEntities1 entities = new DOTNETEntities1();

        // GET: Vegetable
        public ActionResult Index()
        {
            List<Fruit> FruitList = entities.Fruits.ToList();
            return View(FruitList);
        }
        [HttpGet]
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