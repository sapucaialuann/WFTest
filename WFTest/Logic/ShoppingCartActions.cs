using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFTest.Models;

namespace WFTest.Logic
{
    public class ShoppingCartActions: IDisposable
    {
        public string ShoppingCartId { get; set; }
        private ProductContext _db = new ProductContext();
        public const string CartSessionKey = "CartID";
        public void AddToCart(int id)
        {
            ShoppingCartId = GetCartId();

            var cartItem = _db.ShoppingCartItems.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == id);

            if (cartItem == null) {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartId,
                    Product = _db.Products.SingleOrDefault(p => p.ProductId == id),
                    Quantity = 1,
                    CreatedDate = DateTime.Now,
                };

                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
            _db.SaveChanges();
        }
        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }
        public string GetCartId()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name)) 
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;  
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }
        public List<CartItem> GetCartItems() 
        {
            ShoppingCartId = GetCartId();

            return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        }
        public decimal GetTotal()
        {
            ShoppingCartId = GetCartId();
            decimal? total = decimal.Zero;
            total = (decimal?)(from cartItems in _db.ShoppingCartItems
                where cartItems.CartId == ShoppingCartId
                select (int?)cartItems.Quantity * cartItems.Product.ProductPrice).Sum();
            return total ?? decimal.Zero;
        }
        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
        public ShoppingCartActions GetCart(HttpContext context)
        {
            using (var cart = new ShoppingCartActions())
            {
                cart.ShoppingCartId = cart.GetCartId();
                return cart;
            }
        }
        public void RemoveItem(string removeCartId, int removeProductId)
        {
            using (var db = new WFTest.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems
                        where c.CartId == removeCartId && c.Product.ProductId == removeProductId
                        select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        db.ShoppingCartItems.Remove(myItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Remove Cart Item - " + exp.Message.ToString(), exp);
                }
            }
        }
        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            var cartItems = _db.ShoppingCartItems.Where(
                c => c.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                _db.ShoppingCartItems.Remove(cartItem);
            }
            _db.SaveChanges();
        }
        public int GetCount()
        {
            ShoppingCartId = GetCartId();
            
            int? count = (from cartItems in _db.ShoppingCartItems
                where cartItems.CartId == ShoppingCartId
                select (int?)cartItems.Quantity).Sum();
            return count ?? 0;
        }
        public void UpdateItem(string updateCartId, int updateProductId, int quantity)
        {
            using (var db = new WFTest.Models.ProductContext())
            {
                try
                {
                    var myItem =
                        (from c in db.ShoppingCartItems
                            where c.CartId == updateCartId && c.Product.ProductId == updateProductId
                            select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        myItem.Quantity = quantity;
                        db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
                }
            }
        }
        public void UpdateShoppingCartDatabase(String cartId, ShoppingCartUpdates[] cartItemUpdates){
            using (var db = new WFTest.Models.ProductContext())
            {
                try
                {
                    int cartItemCount = cartItemUpdates.Count();
                    List<CartItem> myCart = GetCartItems();
                    foreach (var cartItem in myCart)
                    {
                        for (int i = 0; i < cartItemCount; i++)
                        {
                            if (cartItem.Product.ProductId == cartItemUpdates[i].ProductId)
                            {
                                if (cartItemUpdates[i].PurchaseQuantity < 1 || cartItemUpdates[i].RemoveItem == true)
                                {
                                    RemoveItem(ShoppingCartId, cartItem.ProductId);
                                }
                                else
                                {
                                    UpdateItem(ShoppingCartId, cartItem.ProductId, cartItemUpdates[i].PurchaseQuantity);
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp);
                }
            }
        }
        public void MigrateCart(string cartId, string userName)
        {
            var shoppingCart = _db.ShoppingCartItems.Where(c => c.CartId == cartId);
            foreach (CartItem item in shoppingCart)
            {
                item.CartId = userName;
            }
            HttpContext.Current.Session[CartSessionKey] = userName;
            _db.SaveChanges();
        }
    }
}