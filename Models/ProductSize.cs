using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class ProductSize:BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Nullable<int> SizeId { get; set; }
        public Size Size { get; set; }

    }
}
