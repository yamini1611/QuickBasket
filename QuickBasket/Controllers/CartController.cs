using Antlr.Runtime.Misc;
using Microsoft.AspNet.Identity;
using QuickBasket.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI.WebControls;

namespace QuickBasket.Controllers
{
    public class CartController : Controller
    {
        private readonly DOTNETEntities8  entities = new DOTNETEntities8();

        public ActionResult CartView()
        { 
            List<Cart> cart = entities.Carts.ToList();
            return View(cart);

        }
        public ActionResult Edit(int? id)
        {
            Cart cart = entities.Carts.Find(id);
            return View(cart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(int cartId, bool isIncrement)
        {
            var cart = entities.Carts.Find(cartId);

            if (cart != null)
            {
                if (isIncrement)
                {
                    cart.quantity += 1;
                }
                else
                {
                    if (cart.quantity > 1)
                    {
                        cart.quantity -= 1;
                    }
                }

                cart.price = CalculateUpdatedPrice(cart);

                UpdateStock(cart, isIncrement);

                entities.Entry(cart).State = EntityState.Modified;
                entities.SaveChanges();
            }

            return RedirectToAction("CartView");
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

            return null;
        }

        public void UpdateStock(Cart cart, bool isIncrement)
        {
            if (isIncrement) 
            {
                if (cart.Vegetable != null)
                {
                    cart.Vegetable.stock -= 1;
                }
                else if (cart.Fruit != null)
                {
                    cart.Fruit.stock -= 1;
                }

                else if (cart.Flower != null)
                {
                    cart.Flower.stock -= 1;
                }
                else if (cart.Meat != null)
                {
                    cart.Meat.stock -= 1;

                }
                else if (cart.packedfood != null)
                {
                    cart.packedfood.stock -= 1;
                }

            }
            else 
            {
                if (cart.Vegetable != null)
                {
                    cart.Vegetable.stock += 1;
                }
                else if (cart.Fruit != null)
                {
                    cart.Fruit.stock += 1;
                }
                else if (cart.Flower != null)
                {
                    cart.Flower.stock += 1;
                }
                else if (cart.Meat!= null)
                {
                    cart.Meat.stock += 1;
                }
                else if (cart.packedfood != null)
                {
                    cart.packedfood.stock += 1;
                }


            }
        }


        public ActionResult Delete(int? id)
        {
            Cart cart = entities.Carts.Find(id);

            if (cart == null)
            {
                return HttpNotFound();
            }

            if (cart.Vegetable != null)
            {
                var vegetable = cart.Vegetable;
                vegetable.stock += cart.quantity;
                entities.Carts.Remove(cart);
            }
            else if (cart.Fruit != null)
            {
                var fruit = cart.Fruit;
                fruit.stock += cart.quantity;
                entities.Carts.Remove(cart);
            }
            else if (cart.Flower != null)
            {
                var flower = cart.Flower;
                flower.stock += cart.quantity;
                entities.Carts.Remove(cart);
            }
            else if (cart.Meat != null)
            {
                var meat = cart.Meat;
                meat.stock += cart.quantity;
                entities.Carts.Remove(cart);
            }
            else if (cart.packedfood != null)
            {
                var packedfood = cart.packedfood;
                packedfood.stock += cart.quantity;
                entities.Carts.Remove(cart);
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


