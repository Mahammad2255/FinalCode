using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.ViewModels
{
    public class ReviewVM
    {
        public DateTime CreatedAt { get; set; }
        public int Star { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
