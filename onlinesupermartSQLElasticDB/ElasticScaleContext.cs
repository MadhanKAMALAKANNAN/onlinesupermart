// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//Modified: Madhan KAMALAKANNAN, 21/09/2022
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using onlinesupermartSQLElasticDB.Models;
//using onlinesupermartSQLElasticDB.Models;
//using Microsoft.EntityFrameworkCore;

namespace onlinesupermartSQLElasticDB
{
    public class ElasticScaleContext<T> : System.Data.Entity.DbContext
    {


        // Let's use the standard DbSets from the EF tutorial
       public System.Data.Entity.DbSet<Posts> Posts { get; set; }
        public System.Data.Entity.DbSet<Blogs> Blogs { get; set; }
        //public DbSet<PaymentTypes> PaymentTypes { get; set; }

        //public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        //public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

        //public DbSet<User> Users { get; set; }

        //  public DbSet<Cart> Cart { get; set; }
        // public DbSet<CartDetails> CartDetails { get; set; }
        //public DbSet<ProductsReview> ProductsReview { get; set; }
        //public DbSet<Products> Products { get; set; }


        //public DbSet<Payment> Payment { get; set; }
        //public DbSet<PaymentTypes> PaymentTypes { get; set; }
        //public DbSet<OrderDetails> OrderDetails { get; set; }
        //public DbSet<Orders> Orders { get; set; }
        //public DbSet<Coupon> Coupon { get; set; }
        //public DbSet<ShippingOptions> ShippingOptions { get; set; }

        //public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        //public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }

        //public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

        //public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        //
        //public virtual DbSet<Cart> Cart { get; set; }
        //public virtual DbSet<CartDetails> CartDetails { get; set; }
        //public virtual DbSet<Coupon> Coupon { get; set; }
        //public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        //public virtual DbSet<Orders> Orders { get; set; }
        //public virtual DbSet<Payment> Payment { get; set; }
        //public virtual DbSet<PaymentTypes> PaymentTypes { get; set; }
        //public virtual DbSet<Products> Products { get; set; }
        //public virtual DbSet<ProductsCategories> ProductsCategories { get; set; }
        //public virtual DbSet<ProductsReview> ProductsReview { get; set; }
        //public virtual DbSet<ShippingOptions> ShippingOptions { get; set; }

        // Regular constructor calls should not happen.
        // 1.) Use the protected c'tor with the connection string parameter
        // to intialize a new shard. 
        // 2.) Use the public c'tor with the shard map parameter in
        // the regular application calls with a tenant id.

         string ConnectionString { get; set; }
        // C'tor to deploy schema and migrations to a new shard
        public ElasticScaleContext(string connectionString)
            : base(SetInitializerForConnection(connectionString))
        {
            ConnectionString = connectionString;
        }

        // Only static methods are allowed in calls into base class c'tors
        private static string SetInitializerForConnection(string connnectionString)
        { 
            // We want existence checks so that the schema can get deployed
            Database.SetInitializer<ElasticScaleContext<T>>(new CreateDatabaseIfNotExists<ElasticScaleContext<T>>());
            return connnectionString;
        }

        // C'tor for data dependent routing. This call will open a validated connection routed to the proper
        // shard by the shard map manager. Note that the base class c'tor call will fail for an open connection
        // if migrations need to be done and SQL credentials are used. This is the reason for the 
        // separation of c'tors into the DDR case (this c'tor) and the public c'tor for new shards.
        public ElasticScaleContext(ShardMap shardMap, T shardingKey, string connectionStr)
            : base(CreateDDRConnection(shardMap, shardingKey, connectionStr), true /* contextOwnsConnection */)
        {
        }
       
        // Only static methods are allowed in calls into base class c'tors
        private static DbConnection CreateDDRConnection(ShardMap shardMap, T shardingKey, string connectionStr)
        {
            // No initialization
            Database.SetInitializer<ElasticScaleContext<T>>(null);

            // Ask shard map to broker a validated connection for the given key
            SqlConnection conn = shardMap.OpenConnectionForKey<T>(shardingKey, connectionStr, ConnectionOptions.Validate);
            return conn;
        }
        public ElasticScaleContext(RangeShardMap<int> shardMap, T shardingKey, string connectionStr)
          : base(CreateDDRConnection(shardMap, shardingKey, connectionStr), true /* contextOwnsConnection */)
        {
        }

        private static DbConnection CreateDDRConnection(RangeShardMap<int> shardMap1, T shardingKey, string connectionStr)
        {
            // No initialization
            Database.SetInitializer<ElasticScaleContext<T>>(null); 
            // Ask shard map to broker a validated connection for the given key
            SqlConnection conn = shardMap1.OpenConnectionForKey<T>(shardingKey, connectionStr, ConnectionOptions.Validate);
            return conn;
        }

      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
              new Microsoft.EntityFrameworkCore. DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
            dbContextOptionsBuilder.UseSqlServer(ConnectionString);
            using (var db = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))//To Create Model Context for current ConnectionString (DB)
            {

            }
           // modelBuilder.Entity<PaymentTypes>().Property(e => e.PaymentType).HasMaxLength(50);
           //base.OnModelCreating(modelBuilder);
        }
    }
}