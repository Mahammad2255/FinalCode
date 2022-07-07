using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class Basket: BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> SizeId { get; set; }
        public Product Product { get; set; }
        public Size Size { get; set; }
        public int Count { get; set; }
    }
}
