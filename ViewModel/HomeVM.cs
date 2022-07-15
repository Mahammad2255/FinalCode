using FinalCode.Models;
using FinalCode.ViewModels;
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
        public IEnumerable<Size> Sizes { get; set; }

        public IEnumerable<Review> Reviews { get; set; }
        public List<ReviewVM> ReviewVMs { get; set; }
    }
}
