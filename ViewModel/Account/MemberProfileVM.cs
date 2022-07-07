using FinalCode.ViewModel.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.ViewModel.Account
{
    public class MemberProfileVM
    {
        public MemberUpdateVM Member { get; set; }
        public List<FinalCode.Models.Order> Orders { get; set; }
    }
}
