using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.ViewModel.Basket
{
    public class BasketVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int SizeId { get; set; }
        public double Price { get; set; }
        public double EcoTax { get; set; }
        public double VAT { get; set; }

        public string Image { get; set; }


    }
}
