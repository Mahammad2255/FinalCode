﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.ViewModel.Account
{
    public class MemberUpdateVM
    {
        [StringLength(255)]
        public string FullName { get; set; }
        [StringLength(255), Required]
        public string UserName { get; set; }
        [StringLength(255), Required, EmailAddress]
        public string Email { get; set; }
        [StringLength(255)]
        public string PhoneNumber { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(255)]
        public string Country { get; set; }
        [StringLength(255)]
        public string City { get; set; }
        [StringLength(255)]
        public string State { get; set; }
        [StringLength(255)]
        public string ZipCode { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare(nameof(Password)), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
