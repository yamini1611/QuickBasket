using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuickBasket.Models;
using Razorpay.Api;
using MimeKit;
using MailKit.Net.Smtp;
using QuickBasket.Extensions;
using Microsoft.Owin.BuilderProperties;

namespace QuickBasket.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {

        public DOTNETEntities8 entities = new DOTNETEntities8();


        // GET: Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(int totalAmount, string products, string address)
        {
            int userId = (int)Session["UserID"];
            DateTime possibleDelivery = DateTime.Now.AddHours(2);
            RazorpayClient client = new RazorpayClient("rzp_test_FM7r7jy6LpI1GA", "CwpCT7huCG7slPPd6BZwljEK");

            Dictionary<string, object> options = new Dictionary<string, object>
    {
        { "amount", totalAmount * 100},
        { "currency", "INR" },
    };

            string receiptId = "receipt_" + Guid.NewGuid().ToString().Substring(0, 32);
            options.Add("receipt", receiptId);
            options.Add("payment_capture", "0");
            Razorpay.Api.Order razorpayOrder = client.Order.Create(options);
            string razorpayOrderId = razorpayOrder["id"].ToString();
           

           
                var order = new Models.Order
                {
                    Placeddate = DateTime.Now,
                    Total_amount = totalAmount,
                    products = products,
                    userid = userId,
                    address = address,
                    RazorpayOrderId = razorpayOrderId,
                    Possibledelivery = possibleDelivery
                };

            TempData["address"] = address;
            TempData["Totalamount"] = totalAmount;
            TempData["products"] = products;
            TempData["Razorpayid"] = razorpayOrderId;

               

            return RedirectToAction("OrderPlaced" ,order);
            
        }

        public ActionResult OrderPlaced(Models.Order order)
        {
             return View(order);

        }

        public ActionResult PaymentConfirm(Models.Order order ,string RazorpayOrderId)
        {
            int? TotalAmount = TempData["Totalamount"] as int?;
            string products = TempData["products"] as string;
            string address = TempData["address"] as string;
            string paymentid = Request.Params["rzp_paymentid"];
            RazorpayClient client = new RazorpayClient("rzp_test_FM7r7jy6LpI1GA", "CwpCT7huCG7slPPd6BZwljEK");

            Payment payment = client.Payment.Fetch(paymentid);


            Dictionary<string, object> options = new Dictionary<string, object>
                  {
            { "amount", TotalAmount * 100},
            { "currency", "INR" },
           };

            Payment paymentcaptured = payment.Capture(options);
            string amt = paymentcaptured["amount"].ToString();
            int userId = (int)Session["UserID"];
            DateTime possibleDelivery = DateTime.Now.AddHours(2);

            if (paymentcaptured["status"].ToString() == "captured")
            {
                order.userid = userId;
                order.address = address;
                order.Total_amount = TotalAmount;
                order.products = products;
                order.Placeddate = DateTime.Now;
                order.RazorpayOrderId = RazorpayOrderId;
                order.Possibledelivery = possibleDelivery;

                entities.Orders.Add(order);
                entities.SaveChanges();
                EmptyUserCart(userId);
            }
            else
            {
                this.AddNotification("Payment has Cancelled " ,NotificationType.ERROR);
            }
            return RedirectToAction("OrderConfirmation", new { id = RazorpayOrderId });
        }

        private void EmptyUserCart(int userId)
        {
            var userCartItems = entities.Carts.Where(cart => cart.User.userid == userId).ToList();

            foreach (var cartItem in userCartItems)
            {
                entities.Carts.Remove(cartItem);
            }

            entities.SaveChanges();
        }


        public ActionResult OrderConfirmation(string id) 
        {
            Models.Order order = entities.Orders.SingleOrDefault(c => c.RazorpayOrderId == id);

            if (order != null)
            {
                return View(order);
            }
            else
            {
                return RedirectToAction("Errror400");
            }
        }

        public ActionResult ViewOrder()
        {
              List<Models.Order> orders = entities.Orders.ToList();

            return View(orders);
        }

        [HttpPost]
        public ActionResult SendOrderEmail(string razorpayOrderId)
        {
            try
            {
                var order = entities.Orders.FirstOrDefault(o => o.RazorpayOrderId == razorpayOrderId);

                if (order == null)
                {
                    return RedirectToAction("Errror400");
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Yaminipriya", "20bsca151yaminipriyaj@skacas.ac.in"));
                message.To.Add(new MailboxAddress(order.User.Username, order.User.email)); 
                message.Subject = "Order Confirmation";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = "Thank you for placing your order with us. Your order details are as follows:\n\n" +
                                $"Order ID: {order.RazorpayOrderId}\n" +
                                $"Total Amount: ₹ {order.Total_amount}\n" +
                                $"Products: {order.products}\n" +
                                $"Delivery Address: {order.address}\n" +
                                $"Possible Delivery Date: {order.Possibledelivery}"
                };


                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("20bsca151yaminipriyaj@skacas.ac.in", "Yamini@1611");
                    client.Send(message);
                    client.Disconnect(true);
                }

                this.AddNotification("Email Send Successfully", NotificationType.SUCCESS);

                return RedirectToAction("OrderConfirmation", new { id = razorpayOrderId });
            }
            catch (Exception)
            {
                this.AddNotification("Email Error", NotificationType.ERROR);

                return RedirectToAction("OrderConfirmation"); 
            }
        }
       

    }

    
}
