using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using QuickBasket.Extensions;
using QuickBasket.Models;

namespace QuickBasket.Controllers
{

    public class CartController : Controller
    {
        private readonly DOTNETEntities8 entities = new DOTNETEntities8();

        public ActionResult CartView()
        {
            List<Cart> cart = entities.Carts.ToList();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(int cartId, bool isIncrement)
        {
            var cart = entities.Carts.Find(cartId);

            if (cart != null)
            {
                int? stock = GetStockForCart(cart);

                if (isIncrement)
                {
                    if (stock > 0)
                    {
                        cart.quantity += 1;
                        UpdateStock(cart, -1);
                    }
                    else
                    {
                        TempData["Message"] = "You can't choose a quantity greater than available stock.";
                    }
                }
                else
                {
                    if (cart.quantity > 1)
                    {
                        cart.quantity -= 1;
                        UpdateStock(cart, 1);
                    }
                }

                cart.price = CalculateUpdatedPrice(cart);

                entities.Entry(cart).State = EntityState.Modified;
                entities.SaveChanges();

            }

            return RedirectToAction("CartView");
        }


        private int? GetStockForCart(Cart cart)
        {
            if (cart.Vegetable != null)
            {
                return cart.Vegetable.stock;
            }
            else if (cart.Fruit != null)
            {
                return cart.Fruit.stock;
            }
            else if (cart.Flower != null)
            {
                return cart.Flower.stock;
            }
            else if (cart.packedfood != null)
            {
                return cart.packedfood.stock;
            }
            else if (cart.Meat != null)
            {
                return cart.Meat.stock;
            }

            return 0;
        }

        private int? CalculateUpdatedPrice(Cart cart)
        {
            int? unitPrice = GetUnitPrice(cart);

            return (unitPrice ?? 0) * cart.quantity;
        }

        private int? GetUnitPrice(Cart cart)
        {
            if (cart.Vegetable != null)
            {
                return cart.Vegetable.retailprice;
            }
            else if (cart.Fruit != null)
            {
                return cart.Fruit.retailprice;
            }
            else if (cart.Flower != null)
            {
                return cart.Flower.retailprice;
            }
            else if (cart.packedfood != null)
            {
                return cart.packedfood.retailprice;
            }
            else if (cart.Meat != null)
            {
                return cart.Meat.retailprice;
            }

            return 0;
        }

        public void UpdateStock(Cart cart, int changeAmount)
        {
            if (cart.Vegetable != null)
            {
                cart.Vegetable.stock += changeAmount;

            }
            else if (cart.Fruit != null)
            {
                cart.Fruit.stock += changeAmount;
            }
            else if (cart.Flower != null)
            {
                cart.Flower.stock += changeAmount;
            }
            else if (cart.packedfood != null)
            {
                cart.packedfood.stock += changeAmount;
            }
            else if (cart.Meat != null)
            {
                cart.Meat.stock += changeAmount;
            }
        }

        public ActionResult Delete(int? id)
        {
            Cart cart = entities.Carts.Find(id);

            if (cart == null)
            {
                return RedirectToAction("Errror400");
            }

            if (cart.Vegetable != null)
            {
                var vegetable = cart.Vegetable;
                vegetable.stock += cart.quantity;
                entities.Carts.Remove(cart);
                this.AddNotification("Product Removed from cart", NotificationType.SUCCESS);

            }
            else if (cart.Fruit != null)
            {
                var fruit = cart.Fruit;
                fruit.stock += cart.quantity;
                entities.Carts.Remove(cart);
                this.AddNotification("Product Removed from cart", NotificationType.SUCCESS);

            }
            else if (cart.Flower != null)
            {
                var flower = cart.Flower;
                flower.stock += cart.quantity;
                entities.Carts.Remove(cart);
                this.AddNotification("Product Removed from cart", NotificationType.SUCCESS);

            }
            else if (cart.Meat != null)
            {
                var meat = cart.Meat;
                meat.stock += cart.quantity;
                entities.Carts.Remove(cart);
                this.AddNotification("Product Removed from cart", NotificationType.SUCCESS);

            }
            else if (cart.packedfood != null)
            {
                var packedfood = cart.packedfood;
                packedfood.stock += cart.quantity;
                entities.Carts.Remove(cart);
                this.AddNotification("Product Removed from cart", NotificationType.SUCCESS);

            }
            else
            {
                return HttpNotFound();
            }

            entities.SaveChanges();
            return RedirectToAction("CartView");
        }
    }
}
