using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ar_Shop.Data;
using Ar_Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Ar_Shop.Data.Migrations;

namespace Ar_Shop.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;



        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> AllProducts()
        {
            var products = await _context.Product
            .Include(p => p.Pictures)
            .Include(p => p.Reviews)
            .ToListAsync();
            return View("ProductPages/AllProducts", products);
        }


        [Authorize(Roles = "Admin")]

        // GET: Products
        public async Task<IActionResult> Index()
        {
           /*   return _context.Product != null ? 
                          View(await _context.Product.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Product'  is null.");*/

            var products = await _context.Product
                .Include(p => p.Pictures)
                .Include(p => p.Reviews)
                .ToListAsync();

            // Log the image URLs for each product
            foreach (var product in products)
            {
                _logger.LogInformation($"Product ID: {product.Id}");
                foreach (var picture in product.Pictures)
                {
                    _logger.LogInformation($"Image URL: {picture.Url}");
                }
            }
            return View(products);

        }


        // POST: Products/AddReview
    [HttpPost]
    public async Task<IActionResult> AddReview(int id, string reviewContent)
        {
            var product = await _context.Product
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product != null && !string.IsNullOrEmpty(reviewContent))
            {
                // Create a new Review object and add it to the product's Reviews collection
                var review = new Review
                {
                    Content = reviewContent
                };
                product.Reviews.Add(review);

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }

            // Redirect back to the product details page
            return RedirectToAction("Details", new { id = id });
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Pictures) // Include the related pictures
                .Include(p => p.Reviews) // Include the related reviews
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Product product)
        {
            if (ModelState.IsValid)
            {
                // Process uploaded image files and save as picture URLs
                if (product.PictureFiles != null && product.PictureFiles.Count > 0)
                {
                    foreach (var file in product.PictureFiles)
                    {
                        // Generate a unique filename for each uploaded image
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                        // Combine the unique filename with the server's wwwroot/images directory
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", uniqueFileName);

                        // Save the uploaded image to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create a new Picture object and add it to the product's Pictures collection
                        product.Pictures.Add(new Picture { Url = "/image/" + uniqueFileName });
                    }
                }

                // Save the product with associated pictures to the database
                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Discount,Quantity")] Models.Product product, List<IFormFile>? PictureFiles)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing product from the database, including its associated pictures
                    var existingProduct = await _context.Product
                        .Include(p => p.Pictures)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update other properties (name, price, discount)
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Discount = product.Discount;
                    existingProduct.Quantity= product.Quantity;

                    // Remove existing pictures that need to be deleted (only if new pictures are added)
                    if (PictureFiles != null && PictureFiles.Count > 0)
                    {
                        // Mark all existing pictures for deletion
                        foreach (var picture in existingProduct.Pictures)
                        {
                            picture.NeedToDelete = true;
                        }

                        foreach (var file in PictureFiles)
                        {
                            // Generate a unique filename for each uploaded image
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                            // Combine the unique filename with the server's wwwroot/image directory
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", uniqueFileName);

                            // Save the uploaded image to the server
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Create a new Picture object and add it to the product's Pictures collection
                            existingProduct.Pictures.Add(new Picture { Url = "/image/" + uniqueFileName });
                        }

                        // Remove pictures marked for deletion
                        existingProduct.Pictures.RemoveAll(p => p.NeedToDelete);
                    }

                    // Save the changes to the database
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public IActionResult Search(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                // If the search query is empty, redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            // Perform the search by querying the database for all products
            // and then perform client-side filtering based on the search query
            var products = _context.Product
                .Include(p => p.Pictures)
                .Include(p => p.Reviews)
                .ToList() // Perform client-side evaluation
                .Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Pass the search query and the matching products to the view
            ViewBag.SearchQuery = searchQuery;
            return View("SearchResults", products);
        }


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
       public async Task<IActionResult> DeleteConfirmed(int id)
{
    var product = await _context.Product
        .Include(p => p.Pictures)
        .Include(p => p.Reviews)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (product == null)
    {
        return NotFound();
    }

    // Delete associated images
    foreach (var picture in product.Pictures.ToList())
    {
        _context.Remove(picture);
    }

    // Delete associated reviews
    foreach (var review in product.Reviews.ToList())
    {
        _context.Remove(review);
    }

    // Delete the product
    _context.Product.Remove(product);

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
    }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
