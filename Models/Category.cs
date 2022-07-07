using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class Category: BaseEntity
    {
        [StringLength(255), Required]
        public string Name { get; set; }
        [StringLength(1000)]
        public IEnumerable<Product> Products { get; set; }


    }
}
