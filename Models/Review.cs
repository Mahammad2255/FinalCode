using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Models
{
    public class Review:BaseEntity
    {
        [StringLength(255), Required]
        public string Name { get; set; }
        [StringLength(500), Required, EmailAddress]
        public string Email { get; set; }

        [StringLength(500), Required]
        public string Title { get; set; }
        [MaxLength(5)]
        public int Rate { get; set; }
        public int ProductId { get; set; }
        public Product Product{ get; set; }
    }
}
