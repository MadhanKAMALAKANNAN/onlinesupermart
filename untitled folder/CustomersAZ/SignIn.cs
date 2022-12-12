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
using AuthenticationLibrary;
using Microsoft.IdentityModel.Tokens;

namespace CustomersAZ
{
    public  class SignIn
    {
        [FunctionName("SignIn")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            /*
                       Authentication authentication = new Authentication();
                        authentication.httpContext = req.HttpContext;
                        authentication.httpRequest = req;
                        string name = req.Query["name"]+"";
                        name = name == "" ? "UserOne" : name;
                        var tkn =  authentication.SignIn(name, "pass");

                        //string name = req.Form["name"];
            */
            var tkn = ""; string name = req.Query["name"] + "";
            string responseMessage = tkn==""
                ? "Sign In not Successfull"
                : $"Hello, {name}. user SignedIn successfull";

            return new OkObjectResult(responseMessage);
        }
    }
}
