//Created by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  Nov/2022

using AuthenticationLibrary;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using onlinesupermartSQLElasticDB;
using onlinesupermartSQLElasticDB.Models;

namespace AuthenticationLibraryUnitTest;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Sharding sharding = new Sharding();
        ShardMapManager shardMapManager = sharding.GetShardManager(sharding.config["SQLServerName"], sharding.config["ShardMapManagerDatabaseName"], sharding.config["connectionString"]); //, true, loggerList);
        Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
          new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
        var connectionString = sharding.config["connectionString"];
        var lastDbName = ShardManagementUtils.GetLastShardMapDBName(shardMapManager, sharding.config["shardName"]);
        connectionString = connectionString.Replace("onlinesupermartSQLElasticDB", lastDbName);
        dbContextOptionsBuilder.UseSqlServer(connectionString);
        using(AzureonlinesupermartDbContext azureonlinesupermartDbContext = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))
        {                                                                                                                                                                                                                                                                                                                                                                              
            var httpContext = new DefaultHttpContext();

            Authentication authentication = new Authentication(azureonlinesupermartDbContext, httpContext);

            //Console.WriteLine("Is authentication object not  null:{0}", authentication);
            Assert.IsNotNull(authentication);
            AspNetUsers aspnetUsers = new AspNetUsers();
            aspnetUsers.Email = "AspNetUsers1@email.com";
            aspnetUsers.NormalizedEmail = aspnetUsers.Email;
            aspnetUsers.EmailConfirmed = true;
            aspnetUsers.UserName = aspnetUsers.Email;
            aspnetUsers.TwoFactorEnabled = false;

            //new user regist test
           var newUserRegOk  = authentication.RegisterUserOk(aspnetUsers,"Pas");
          if(newUserRegOk)
            {
                Assert.IsTrue(newUserRegOk);
                Console.WriteLine("RegisterUserOk Successfull for username :{0}, password: {1}  ", aspnetUsers.Email, "Pas");
            }
            else
            {
                Assert.IsFalse(newUserRegOk);
                Console.WriteLine("RegisterUserOk fails,since{0} aleady exists ", aspnetUsers.Email);
            }
            //signin Pass test with correct password
            var token = authentication.SignIn(aspnetUsers.UserName, "Pas");
            Console.WriteLine("User  :{0} IsAuthenticated :  {1} ", aspnetUsers.UserName, authentication.IsAuthenticated);
            if (authentication.IsAuthenticated)
            {

                Assert.IsTrue(authentication.IsAuthenticated);
            }
            else
            {
                Assert.IsFalse(authentication.IsAuthenticated);
            }
            if (token != null)
            { 
                Assert.IsNotNull(token);
                Console.WriteLine("SignIn Successfull for username :{0}, password: {1} with tokens={2}   , token partial : {3}:", aspnetUsers.UserName, "Pas", token.CipherToken, token.TokenPart);

               
                //token validity test
                var isTokenValid = authentication.IsTokenValid(token);
                Console.WriteLine("Is Token Valid?{0}" + isTokenValid);
                Assert.IsTrue(isTokenValid);
            }
            else
            {
                Assert.IsNull(token);
            } 

            //SignOut Success test
            var signOut = authentication.SignOut();
            Console.WriteLine("Is SignOut Successfull:{0}", signOut);
            Assert.IsTrue(signOut);

            //signin Fail test
            token = authentication.SignIn(aspnetUsers.UserName, "Pasqwe");
            Console.WriteLine("User  :{0} IsAuthenticated :  {1} ", aspnetUsers.UserName, authentication.IsAuthenticated);
            if (authentication.IsAuthenticated)
            {

                Assert.IsTrue(authentication.IsAuthenticated);
            }
            else
            {
                Assert.IsFalse(authentication.IsAuthenticated);
            }

            if (token != null)
            {
                Console.WriteLine("SignIn Successfull for username :{0}, password: {1} with tokens={2}   , token partial : {3}:", aspnetUsers.UserName, "Pasqew",token.CipherToken, token.TokenPart);

            }
            else
            {
                Assert.IsNull(token); 
                Console.WriteLine("SignIn fails for username:{0}, password: {1} with no tokens ", aspnetUsers.UserName, "Pasqwe");
            }
        }
    }
}
 