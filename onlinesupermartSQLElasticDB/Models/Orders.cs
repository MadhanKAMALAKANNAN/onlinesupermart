﻿//Created by: Madhan KAMALAKANNAN,  Aug/2022
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace onlinesupermartSQLElasticDB.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrdersDetails = new HashSet<OrdersDetails>();
            Payments = new HashSet<Payments>();
        }

        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public decimal DateTime { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<OrdersDetails> OrdersDetails { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
    }
}