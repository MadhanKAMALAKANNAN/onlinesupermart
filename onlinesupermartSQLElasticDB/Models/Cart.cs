﻿//Created by: Madhan KAMALAKANNAN,  Aug/2022
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace onlinesupermartSQLElasticDB.Models
{
    public partial class Cart
    {
        public Cart()
        {
            CartDetails = new HashSet<CartDetails>();
            Payments = new HashSet<Payments>();
        }

        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? DateTime { get; set; }

        public virtual ICollection<CartDetails> CartDetails { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
    }
}