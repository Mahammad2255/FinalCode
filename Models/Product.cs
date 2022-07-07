using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class Product:BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; } = "";
        [StringLength(255)]
        public string Name { get; set; } 
        [Column(TypeName="money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DiscountPrice { get; set; }
        public bool Aviability{ get; set; }
        public bool IsBestSeller{ get; set; }
        [Column(TypeName = "money")]
        public double EcoTax { get; set; }
      
        public int CategoryId { get; set; }
        [StringLength(1000), Required]
        public string Description { get; set; }
        public int Count { get; set; }

        public string MainImage { get; set; }
        public bool Availability { get; set; }

        public Category Category { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        [NotMapped]

        public IFormFile MainImageFile { get; set; }
        [NotMapped]
        public IEnumerable<Review> Reviews { get; set; }
        

        public IEnumerable<ProductSize> ProductSizes { get; set; }

        [NotMapped]
        public List<int> SizeIds { get; set; } = new List<int>();
      
        [NotMapped]
      
      
        public IEnumerable<Basket> Baskets { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }

    }
}
