using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class Size: BaseEntity
    {
      
        [Required, StringLength(255)]
        public string Name { get; set; }
        public IEnumerable<ProductSize> ProductSizes { get; set; }

        public IEnumerable<Basket> Baskets { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
