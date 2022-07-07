using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; }

        public IEnumerable<ProductSize> ProductSizeColors{ get; set; }
    }
}
