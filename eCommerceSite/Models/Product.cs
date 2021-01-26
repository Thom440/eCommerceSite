using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceSite.Models
{
    /// <summary>
    /// A salable product
    /// </summary>
    public class Product
    {
        [Key] // Make Primary Key in database
        public int ProductID { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The retail price of the product
        /// </summary>
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        /// <summary>
        /// The category that the product falls into
        /// </summary>
        public string Category { get; set; }
    }
}
