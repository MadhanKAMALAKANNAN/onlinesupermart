using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using AuthenticationLibrary;
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
            var config = SqlDatabaseUtils.GetConfigaration();
            //Sharding sharding = null; Shard shard = null;
            //ShardingEntryClass shardingEntryClass = new ShardingEntryClass();
            //shardingEntryClass.ShardsFunction(out sharding, out shard, config);

          //  builder.Services.AddDbContext<AzureonlinesupermartDbContext>(opt => { opt.UseSqlServer(config["connectionString00"], o => o.EnableRetryOnFailure()); });

            //builder.Services.AddIdentityCore<IdentityUser>(options => { options.SignIn.RequireConfirmedAccount = true; }).AddRoles<IdentityRole>().AddEntityFrameworkStores<AzureonlinesupermartDbContext>().AddDefaultTokenProviders(); ;
            //builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            //{
            //    // enables immediate logout, after updating the user's stat.
            //    options.ValidationInterval = TimeSpan.Zero;
            //});
            //builder.Services.AddHttpContextAccessor();
            //// Identity services
            //builder.Services.TryAddScoped<IUserValidator<IdentityUser>, UserValidator<IdentityUser>>();
            //builder.Services.TryAddScoped<IPasswordValidator<IdentityUser>, PasswordValidator<IdentityUser>>();
            //builder.Services.TryAddScoped<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();
            //builder.Services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            //builder.Services.TryAddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
            //// No interface for the error describer so we can add errors without rev'ing the interface
            //builder.Services.TryAddScoped<IdentityErrorDescriber>();
            //builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<IdentityUser>>();
            //builder.Services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<IdentityUser>>();
            //builder.Services.TryAddScoped<IUserClaimsPrincipalFactory<IdentityUser>, UserClaimsPrincipalFactory<IdentityUser, IdentityRole>>();
            //builder.Services.TryAddScoped<UserManager<IdentityUser>>();
            //builder.Services.TryAddScoped<SignInManager<IdentityUser>>();
            //builder.Services.TryAddScoped<Shard>();
            
            //builder.Services.AddScoped<Authentication>();

            Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
           new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
            dbContextOptionsBuilder.UseSqlServer(SqlDatabaseUtils.GetConfigaration()["connectionString00"]);
            // AzureonlinesupermartDbContext azureonlinesupermartDbContext
            builder.Services.AddScoped<AzureonlinesupermartDbContext>(p=>new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options));//dbContextOptionsBuilder);
            //builder.Services.AddFunctionAuthentication();
            // builder.Services.AddAuthorization();
            // builder.Services.Configure<PasswordHasherOptions>(options => options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3);
            //builder.Services.AddIdentityCore<IdentityUser>(opt =>
            //{
            //    opt.Password.RequireDigit = false;
            //    opt.Password.RequireLowercase = false;
            //    opt.Password.RequireNonAlphanumeric = false;
            //    opt.Password.RequireUppercase = false;
            //})
            // .AddSignInManager()
            // .AddEntityFrameworkStores<AsnetidentityContext>();


            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            //    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            //    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;// ExternalScheme;
            //}).AddIdentityCookies();

            //    .AddGoogle(options =>
            ////{

            ////    options.ClientId = arrayKP.FirstOrDefault(x => x.Key == "Authentication:Google:ClientId").Value;
            ////    options.ClientSecret = arrayKP.FirstOrDefault(x => x.Key == "Authentication:Google:ClientSecret").Value;
            ////    //options.ReturnUrlParameter = "/signin-google";
            ////    //options.CallbackPath = "/signin-google";

            ////});
            //.AddMicrosoftAccount(options =>
            // {

            //     options.ClientId = arrayKP.FirstOrDefault(x => x.Key == "Authentication:Microsoft:ClientId").Value;
            //     options.ClientSecret = arrayKP.FirstOrDefault(x => x.Key == "Authentication:Microsoft:ClientSecret").Value;
            // });


            //builder.Services.AddAuthorization(options =>
            //{
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //    //options.AddPolicy("SiteAdmin", policy => policy.RequireRole("Admin"));

            //});
            //builder.Services.TryAddScoped<RoleManager<IdentityRole>>();


            //builder.Services.u
            //builder.Services.Addd.AddSingleton<IMyService>((s) => {
            //    return new MyService();
            //  });

            //  builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
    
}
