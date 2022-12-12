using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AuthenticationLibrary;
using onlinesupermartSQLElasticDB.Models;
using System.Net.Http;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using onlinesupermartSQLElasticDB;

namespace CustomersAZ
{
    public static class SignOut
    {
        [FunctionName("SignOut")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Sharding sharding = new Sharding();
            ShardMapManager shardMapManager = sharding.GetShardManager(sharding.config["SQLServerName"], sharding.config["ShardMapManagerDatabaseName"], sharding.config["connectionString"]); //, true, loggerList);
            Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
              new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
            var connectionString = sharding.config["connectionString"];
            var lastDbName = ShardManagementUtils.GetLastShardMapDBName(shardMapManager, sharding.config["shardName"]);
            connectionString = connectionString.Replace("onlinesupermartSQLElasticDB", lastDbName);
            dbContextOptionsBuilder.UseSqlServer(connectionString);
            using (AzureonlinesupermartDbContext azureonlinesupermartDbContext = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))
            {
                var httpContext = new DefaultHttpContext();

                Authentication authentication = new Authentication(azureonlinesupermartDbContext, httpContext);

                log.LogInformation("C# HTTP trigger function processed a request.");
         
                authentication.httpContext = req.HttpContext;
                authentication.httpRequest = req;
                var isSignedOut = authentication.SignOut();
                string responseMessage = isSignedOut
                              ? "Signed Out not Successfull"
                              : $"Hello, user Signed Out successfull";

                return new OkObjectResult(responseMessage);
            }
        }
    }
}

