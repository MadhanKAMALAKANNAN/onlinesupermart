﻿//Created by: Madhan KAMALAKANNAN,  Aug/2022
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace onlinesupermartSQLElasticDB.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            CartDetails = new HashSet<CartDetails>();
        }

        public int Id { get; set; }
        public int? CouponName { get; set; }
        public decimal CouponValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}