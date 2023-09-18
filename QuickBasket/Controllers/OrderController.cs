using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuickBasket.Models;
using Razorpay.Api;

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
            DateTime possibleDelivery = DateTime.Now.AddDays(2);
            Razorpay.Api.RazorpayClient client = new RazorpayClient("rzp_test_FM7r7jy6LpI1GA", "CwpCT7huCG7slPPd6BZwljEK");

            Dictionary<string, object> options = new Dictionary<string, object>
    {
        { "amount", totalAmount * 100},
        { "currency", "INR" },
    };
            string receiptId = "receipt_" + Guid.NewGuid().ToString().Substring(0, 32);
            options.Add("receipt", receiptId);

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

            entities.Orders.Add(order);
            entities.SaveChanges();

            // Empty the user's cart after a successful order placement
            EmptyUserCart(userId);

            return View(order);
        }

        private void EmptyUserCart(int userId)
        {
            // Assuming you have access to the CartController
            var cartController = new CartController();

            // Call the UpdateStock method from CartController
            var userCartItems = entities.Carts.Where(cart => cart.User.userid == userId).ToList();
            foreach (var cartItem in userCartItems)
            {
                cartController.UpdateStock(cartItem, true);
            }

            entities.Carts.RemoveRange(userCartItems);
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
                return Content("Order not found");
            }
        }

        public ActionResult ViewOrder()
        {
              List<Models.Order> orders = entities.Orders.ToList();

            return View(orders);
        }


    }
}
