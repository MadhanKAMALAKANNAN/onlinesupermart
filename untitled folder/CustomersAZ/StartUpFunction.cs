//Created by: Madhan KAMALAKANNAN,  Aug/2022
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;

using AuthenticationLibrary;
/*
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using onlinesupermartSQLElasticDB;
using onlinesupermartSQLElasticDB.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
*/
[assembly: FunctionsStartup(typeof(CustomersAZ.Startup))]

namespace CustomersAZ
{
    //public static class Function
    //{
    //    [FunctionName("Function")]
    //    public static async Task<IActionResult> Run(
    //        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    //        ILogger log)
    //    {
    //        log.LogInformation("C# HTTP trigger function processed a request.");

    //        string name = req.Query["name"];

    //        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //        dynamic data = JsonConvert.DeserializeObject(requestBody);
    //        name = name ?? data?.name;

    //        string responseMessage = string.IsNullOrEmpty(name)
    //            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
    //            : $"Hello, {name}. This HTTP triggered function executed successfully.";

    //        return new OkObjectResult(responseMessage);
    //    }
    //}

        public class Startup : FunctionsStartup
        {
        public override void Configure(IFunctionsHostBuilder builder)
            {
          /*     Authentication authentication = new Authentication();

          //authentication.SignIn("test","pass");
            var tkn = authentication.SignIn("test", "pass");
            //string name = req.Query["name"];
            var isvalid = authentication.IsTokenValid(tkn);
             var config = SqlDatabaseUtils.GetConfigaration();


               builder.Services.AddScoped<Authentication>();

               Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
                       new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
               dbContextOptionsBuilder.UseSqlServer(SqlDatabaseUtils.GetConfigaration()["connectionString00"]);

             builder.Services.AddScoped<AzureonlinesupermartDbContext>(p=>new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options));//dbContextOptionsBuilder);

        */
        }
    }
    
}
