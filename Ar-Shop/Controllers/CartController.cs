using Ar_Shop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Ar_Shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CartController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            List<CartItem> cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = dbContext.Product.Find(productId);

            if (product != null)
            {
                if (quantity <= 0 || quantity > product.Quantity)
                {
                    // The requested quantity is not valid or exceeds the available quantity.
                    // You may want to handle this scenario based on your business requirements.
                    TempData["ErrorMessage"] = "The requested quantity is not available for this product.";

                    return RedirectToAction("AllProducts", "Products");
                }

                List<CartItem> cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

                var existingCartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

                if (existingCartItem != null)
                {
                    // If the product is already in the cart, update the quantity
                    existingCartItem.Quantity = quantity;

                }
                else
                {
                    // If the product is not in the cart, add it as a new cart item
                    var cartItem = new CartItem
                    {
                        ProductId = productId,
                        ProductName = product.Name,
                        Price = product.Price,
                        Discount = product.Discount, // Include the discount for the cart item
                        Quantity = quantity
                    };

                    // Calculate the discounted price
                    cartItem.DiscountedPrice = cartItem.Price - (cartItem.Price * (cartItem.Discount / 100));

                    cartItems.Add(cartItem);
                }

                HttpContext.Session.Set("Cart", cartItems);
            }
            TempData["Added to Cart"] = "Producted Added To Cart.";


            return RedirectToAction("AllProducts", "Products");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                HttpContext.Session.Set("Cart", cartItems);
            }

            return RedirectToAction("AllProducts", "Products");
        }

  
        
        public IActionResult check()
        {
            // Handle payment processing with the chosen payment gateway
            // If the payment is successful, continue to update the quantities and clear the cart

            List<CartItem> cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cartItems != null && cartItems.Count > 0)
            {
                // Update quantities in the database and reset the cart
                foreach (var cartItem in cartItems)
                {
                    var product = dbContext.Product.Find(cartItem.ProductId);
                    if (product != null)
                    {
                        if (cartItem.Quantity <= product.Quantity)
                        {
                            product.Quantity -= cartItem.Quantity;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            // Handle insufficient quantity scenario based on your requirements
                            // For example, show an error message to the user, revert the payment, etc.
                        }
                    }
                }

                // Clear the cart after updating the database
                HttpContext.Session.Remove("Cart");
            }
            TempData["Success"] = "Purchase Successfull";

            // Redirect the user to a confirmation page or order summary page
            return RedirectToAction("AllProducts", "Products");
        }
    }


    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } // Include the discount property
        public decimal DiscountedPrice { get; set; } // Include the discounted price property
        public int Quantity { get; set; }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }
}

