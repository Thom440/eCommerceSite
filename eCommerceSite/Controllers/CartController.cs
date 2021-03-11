using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceSite.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductContext _context;
        private readonly IHttpContextAccessor _httpContext;
        public CartController(ProductContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="id">The id of the product to add</param>
        public async Task<IActionResult> Add(int id, string previousUrl)
        {
            // Get product from the database
            Product p = await ProductDB.GetProductAsync(_context, id);

            CookieHelper.AddProductToCart(_httpContext, p);

            TempData["Message"] = p.Title + " added successfully";

            // Redirect back to previous page
            return Redirect(previousUrl);
        }

        /// <summary>
        /// Displays a view to show all products in the shopping cart
        /// </summary>
        public IActionResult Summary()
        {
            List<Product> cartProducts = CookieHelper.GetCartProducts(_httpContext);
            return View(cartProducts);
        }
    }
}
