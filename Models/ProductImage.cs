﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class ProductImage:BaseEntity
    {
        [StringLength(1000), Required]
        public string Image { get; set; }
        public int ProductId { get; set; }
        public Product Product{ get; set; }
    }
}
