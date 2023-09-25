using Newtonsoft.Json;
using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QuickBasket.Controllers;
using System.IO;
using QuickBasket.Extensions;

namespace QuickBasket.Controllers
{
    public class FeedbackController : Controller
    {
        // GET: Feedback
        private readonly DOTNETEntities8 entities = new DOTNETEntities8();
        string Baseurl = "https://localhost:44331/";

        [Authorize(Roles="Admin")]
        public async Task<ActionResult> Index()
        {
            List<Feedback> feed = new List<Feedback>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Feedbacks");
                if (Res.IsSuccessStatusCode)
                {
                    var FeedResponse = Res.Content.ReadAsStringAsync().Result;
                    feed = JsonConvert.DeserializeObject<List<Feedback>>(FeedResponse);
                }
                return View(feed);
            }

        }

        [HttpGet]
        public ActionResult Feedback()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
         
                using (var context = new DOTNETEntities8())
                {
                    int? userid = Session["UserId"] as int?;
                    feedback.userid = userid;
                    context.Feedbacks.Add(feedback);
                    context.SaveChanges();
                }
                this.AddNotification("Thank you for submitting your feedback ", NotificationType.SUCCESS);
               
            }
            return RedirectToAction("Index", "Home");

        }


        public ActionResult Delete(int? id)
        {
            Feedback feed = entities.Feedbacks.Find(id)
    ;
            entities.Feedbacks.Remove(feed);
            entities.SaveChanges();
            return RedirectToAction("Index");
        }

    }

   
}
