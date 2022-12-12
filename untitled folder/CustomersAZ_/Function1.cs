/*Madhan KAMALAKANNAN 10/Octaber/2021 */

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using onlinesupermartSQLElasticDB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Data;
using onlinesupermartSQLElasticDB.Migrations;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration; 
//using Microsoft.AspNet.Identity;
 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
 
using onlinesupermartSQLElasticDB.Models;
using Microsoft.EntityFrameworkCore;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using AuthenticationLibrary;
using System.Threading;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

namespace CustomersAZ
{
    public class UserRegistration
    {
 
       
        private readonly ILogger<UserRegistration> logger;
        private readonly Microsoft.AspNetCore.Identity.UI.Services.IEmailSender emailSender; 
        Sharding sharding = null; Shard shard = null; 
        AzureonlinesupermartDbContext azureonlinesupermartDbContext;
        Authentication authentication;
        IUserStore<IdentityUser> store;
        public UserRegistration(Authentication authentication, AzureonlinesupermartDbContext azureonlinesupermartDbContext)//, UserManager<IdentityUser> userManager )//, Sharding sharding, Shard shard)
        {
            this.authentication = authentication;
            this.azureonlinesupermartDbContext = azureonlinesupermartDbContext;
            authentication.azureonlinesupermartDbContext = azureonlinesupermartDbContext; 
            
        }
 

        [FunctionName("UserRegistration")]
      //  [Authorize]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

           
          log.LogInformation("C# HTTP trigger function processed a request.");
            
            string uname = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            uname = uname ?? data?.name;
            var config = SqlDatabaseUtils.GetConfigaration();

            string s_userName = config["sqlUserName"];
            string s_password = config["sqlPassword"];
            string s_server = config["SQLServerName"];
            string s_shardmapmgrdb = config["ShardMapManagerDatabaseName"];
            string connectionString = config["connectionString"];
           

            //**********************Creates New ShardMap, Shard if needed*****if its new shard then create Table Schemas and reference keys******************
          
            Sharding sharding = null; Shard shard = null;
            ShardingEntryClass shardingEntryClass = new ShardingEntryClass();
            shardingEntryClass.ShardsFunction(out sharding, out shard, config);
            IdentityUser identityUser = null;
            var nm = "UserName" + (azureonlinesupermartDbContext.AspNetUsers.Count());
            var password = "Password@2";
 
           AspNetUsers aspNetUsersModel = new AspNetUsers();
           aspNetUsersModel.EmailConfirmed = true;
           aspNetUsersModel.UserName = nm + "@azId.com";
           aspNetUsersModel.NormalizedUserName = nm + "@azId.com";
           aspNetUsersModel.Email = nm + "@azId.com";
           authentication.httpRequest = req;
           authentication.httpContext = req.HttpContext;
           authentication.identityUser = aspNetUsersModel;
           authentication.SignIn(password);

           //authentication.RegisterUserOk( aspNetUsersModel, password);

           
            //  ClaimsPrincipal claimsPrincipal = userClaimsPrincipalFactory.CreateAsync(user).Result;
            //var usrId =  await store.GetUserIdAsync(identityUser1,CancellationToken.None);

          //var usr1 = this.store.FindByIdAsync(usrId, CancellationToken.None).Result;

            //return new OkResult();


            //////foreach (var claim in additionalClaims)
            //////{
            //////    userPrincipal.Identities.First().AddClaim(claim);
            //////}



            //SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder
            //{
            //    UserID = config["sqlUserName"],
            //    Password = config["sqlPassword"],
            //    ApplicationName = config["AppName"]
            //};
            //connStrBldr.DataSource = shard.Location.DataSource; // s_server;"onlinesupermartSQLElasticDBShard0";//config["SQLServerName"];//
            //connStrBldr.InitialCatalog = shard.Location.Database;//"onlinesupermartSQLElasticDBShard0";//

            //int cnt = 0;
            //DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
            //   new DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
            //dbContextOptionsBuilder.UseSqlServer(connStrBldr.ConnectionString);
            //// AsnetidentityContext.AddBaseOptions(dbContextOptionsBuilder, connectionString);

            //SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            //{
            //    using (var db = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))
            //    {
            //        try
            //        {
            //            cnt = (from b in db.Blogs select b).Count();
            //            if (cnt > 0)
            //            { var maxRange = (from b in db.Blogs select b).Max(m => m.BlogId); }
            //        }
            //        catch (Exception ex) { }
            //    }
            //});




            //using (var asnetidentityContext1 = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options)) //new AsnetidentityContext(connectionString);
            //{

            //    Blogs blog1 = new Blogs();
            //    Blogs blog2 = new Blogs();
            //    // blog.Url = "sdfsdf";
            //    blog1.Name = "blog1";
            //    blog2.Name = "blog2";

            //    asnetidentityContext1.Blogs.Add(blog1); asnetidentityContext1.Blogs.Add(blog2);
            //    asnetidentityContext1.SaveChanges();
            //}
            //IList<Claim> additionalClaims = Array.Empty<Claim>();
            //var authenticationMethod = "Password";

            //if (authenticationMethod != null)
            //{
            //    additionalClaims = new List<Claim>();
            //    additionalClaims.Add(new Claim(ClaimTypes.Name, user.Id));
            //}

            //foreach (var claim in additionalClaims)
            //{
            //    userPrincipal.Identities.First().AddClaim(claim);
            //}

            //var userPrincipal = userClaimsPrincipalFactory.CreateAsync(user.Result).Result;
            ////IUserStore<IdentityUser> _store = new UserStore<IdentityUser>(new AzureonlinesupermartDbContext());
            ////UserManager<IdentityUser> _userManager = new UserManager<IdentityUser>(_store, null, new PasswordHasher<IdentityUser>(), null, null, null, null, null, null);
            ////SignInManager<IdentityUser> signInManager = new SignInManager<IdentityUser>(_userManager, null, null, null, null, null,null);
            //var user = userManager.GetUserAsync(userPrincipal);// new IdentityUser { UserName = "Useremail", Email = "User@email.com" };

            ////// var user = new AspNetUsers();// { UserName = "Useremail.com", Email = "User@email.com" };
            ////user.Email = "User@email.com";
            ////user.UserName = "Useremail";
            //signInManager.PasswordSignInAsync("Password@1",)
            ////var identityResult = signInManager.UserManager.CreateAsync(
            //new IdentityUser()
            //{
            //    Id = "eb4127fe-1fc0-4697-98ce-b1c5e8c93578",
            //    UserName = "User@email.com",
            //    EmailConfirmed = true,
            //}, "Password@1").GetAwaiter().GetResult();

            //new Claim(ClaimTypes.AuthenticationMethod, twoFactorInfo.LoginProvider)




            //if (userManager.FindByNameAsync(user.UserName).Result != null)
            //{
            //    await signInManager.SignInAsync(user, isPersistent: false);

            //}
            //else
            //{
            //    var identityResult = await userManager.CreateAsync(user, "Password@1");
            //    if (identityResult.Succeeded)
            //    {
            //        await signInManager.SignInAsync(user, isPersistent: false);
            //    }
            //}



            //**********************Do User Registration ,Logins**********************
            string responseMessage = "Ok";
            //if (shard!=null&& sharding!=null)
            //{
            //    SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder
            //    {
            //        UserID = config["sqlUserName"],
            //        Password = config["sqlPassword"],
            //        ApplicationName = config["AppName"]
            //    };
            //    connStrBldr.DataSource = shard.Location.DataSource; // s_server;
            //    connStrBldr.InitialCatalog = shard.Location.Database;

            //    SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            //    {
            //        // Go into a DbContext to trigger migrations and schema deployment for the new shard.
            //        // This requires an un-opened connection.
            //        using (var db = new ElasticScaleContext<int>(connStrBldr.ConnectionString))
            //        {
            //            try
            //            {
            //                int cnt = (from b in db.Blogs select b).Count();
            //                if (cnt > 0)
            //                { var maxRange = (from b in db.Blogs select b).Max(m => m.BlogId); }
            //            }
            //            catch (Exception ex) { }
            //        }
            //    });
            //}
            //else
            {
                //responseMessage = "Error, Try again! ";
            }
             
           // string responseMessage = string.IsNullOrEmpty(name)
           //  ? "DB tables are Created"
           //: $"Hello, {name}. This HTTP triggered function executed successfully.";
           //string responseMessage = string.IsNullOrEmpty(name)
           //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
           //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //***************************************************************************************
            return new OkObjectResult(responseMessage);
        }
        
       
    }
}
 