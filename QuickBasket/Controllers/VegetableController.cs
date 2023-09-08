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

    public class VegetableController : Controller
    {
        public DOTNETEntities1 entities = new DOTNETEntities1();

        // GET: Vegetable
        public ActionResult Index()
        {
            List<Vegetable> vegetableList = entities.Vegetables.ToList();
            return View(vegetableList);
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