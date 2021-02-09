using eCommerceSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceSite.Data
{
    public static class ProductDB
    {
        /// <summary>
        /// Returns the total count of products
        /// </summary>
        /// <param name="_context">Database context to use</param>
        public static async Task<int> GetTotalProductsAsync(ProductContext _context)
        {
            return await(from p in _context.Products
                  select p).CountAsync();
        }

        /// <summary>
        /// Get page worth of products
        /// </summary>
        /// <param name="_context">Database context</param>
        /// <param name="pageSize">Number of products per page</param>
        /// <param name="pageNum">Page of products to return</param>
        public static async Task<List<Product>> GetProductsAsync(ProductContext _context, int pageSize, int pageNum)
        {
            return await (from p in _context.Products
                                            orderby p.Title ascending
                                            select p)
                                            .Skip(pageSize * (pageNum - 1))
                                            .Take(pageSize)
                                            .ToListAsync();
        }

        /// <summary>
        /// Adds a product to the database
        /// </summary>
        /// <param name="_context">Database context</param>
        /// <param name="p">A Product to add to database</param>
        /// <returns></returns>
        public static async Task<Product> AddProductAsync(ProductContext _context, Product p)
        {
            // Add to database
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
            return p;
        }
    }
}
