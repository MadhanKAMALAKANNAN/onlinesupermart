//Created by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  DEC/2022

using AuthenticationLibrary;
 
using onlinesupermartSQLElasticDB;
using System;
using ShardingSimulationTimer;

using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using onlinesupermartSQLElasticDB;
using onlinesupermartSQLElasticDB.Models;
using ShardingSimulationTimer;
using AuthenticationLibrary;

namespace ShardingUnitTest;

[TestClass]
public class UnitTest1
{

    static ShardingSimulationTimerClass sstCls;
    //*******
    [TestMethod]
    public void TestMethod1()
    {
        
        sstCls = new ShardingSimulationTimerClass();
        ShardingTest();

        Console.ReadLine();

    }

    public static void ShardingTest()
    {
        Sharding sharding = new Sharding(); var lastDbName1 = "";
      //  do
        {
     
             

            ShardMapManager shardMapManager = sharding.GetShardManager(sharding.config["SQLServerName"], sharding.config["ShardMapManagerDatabaseName"], sharding.config["connectionString"]); //, true, loggerList);
            Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
              new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
            var connectionString = sharding.config["connectionString"];
            var lastDbName = ShardManagementUtils.GetLastShardMapDBName(shardMapManager, sharding.config["shardName"]);
            connectionString = connectionString.Replace("onlinesupermartSQLElasticDB", lastDbName);
            dbContextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(45), errorNumbersToAdd: null);
            });

            var shardingName = lastDbName;
            if (sstCls.NoNewSharding)
            {
                Assert.IsTrue(sstCls.NoNewSharding); //no New sharding
                Console.WriteLine("No new sharding ");
            }
            else

            {
                Assert.IsFalse(sstCls.NoNewSharding); //new sharding
                Console.WriteLine("new sharding got created : {0}", shardingName);
            }

            using (AzureonlinesupermartDbContext azureonlinesupermartDbContext = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))
            {
                var httpContext = new DefaultHttpContext();

                Authentication authentication = new Authentication(azureonlinesupermartDbContext, httpContext);

                //Console.WriteLine("Is authentication object not  null:{0}", authentication); 
                AspNetUsers aspnetUsers = new AspNetUsers();
                var rndm = new Random().NextInt64();////azureonlinesupermartDbContext.AspNetUsers.Count() + 1;

                aspnetUsers.Email = "AspNetUsers" + rndm + "@email.com";
                aspnetUsers.NormalizedEmail = aspnetUsers.Email;
                aspnetUsers.EmailConfirmed = true;
                aspnetUsers.UserName = aspnetUsers.Email;
                aspnetUsers.TwoFactorEnabled = false;
                //create blogid
                Blogs blog = new Blogs();
                blog.Name = "blg" + rndm;
                azureonlinesupermartDbContext.Blogs.Add(blog);
                azureonlinesupermartDbContext.SaveChanges(true);
              
                //
                //create user (if user number more than 2 then) check new sharding got created
                var newUserRegOk = authentication.RegisterUserOk(aspnetUsers, "Pas");
                //new user regist test

                if (newUserRegOk)
                {
                    //Assert.IsTrue(newUserRegOk);
                    Console.WriteLine("RegisterUserOk Successfull for username :{0}, password: {1}  ", aspnetUsers.Email, "Pas");
                    Assert.IsTrue(newUserRegOk);
                }
                else
                {
                    //Assert.IsFalse(newUserRegOk);
                    Console.WriteLine("RegisterUserOk fails,since{0} aleady exists ", aspnetUsers.Email);
                    Assert.IsFalse(newUserRegOk);
                }

                //ShardingSimulationTimer
               // Console.WriteLine("do you want to create another user (yes=)");
            }
        }
       // while (Console.ReadLine() == "y");


    }
}
