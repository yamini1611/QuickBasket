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
        public DOTNETEntities1 entities = new DOTNETEntities1();
        public ActionResult Index()
        {
            List<Flower> FruitList = entities.Flowers.ToList();
            return View(FruitList);
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