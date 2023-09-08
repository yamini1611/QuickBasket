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
    public class MeatController : Controller
    {
        // GET: Meat
        public DOTNETEntities1 entities = new DOTNETEntities1();
        public ActionResult Index()
        {
            List<Meat> MeatList = entities.Meats.ToList();
            return View(MeatList);
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
                entities.Meats.Add(meat);
                entities.SaveChanges();
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
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Meat meat = entities.Meats.Find(id);
            return View(meat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mid ,name ,originalcost ,retailprice ,stock ,image ,offer,category")] Meat meat, HttpPostedFileBase imageFile)
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