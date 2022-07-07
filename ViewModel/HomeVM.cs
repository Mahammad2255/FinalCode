using FinalCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.ViewModel
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public Setting Setting{ get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories{ get; set; }
        public Product Product { get; set; }
    }
}
