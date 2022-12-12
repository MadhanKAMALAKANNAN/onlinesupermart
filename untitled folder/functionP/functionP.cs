using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting.Server;

namespace functionP
{
    public static class functionP
    {
        [FunctionName("functionP")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //docker exec -t mssqlOnlineSuperMart cat /var/opt/mssql/log/errorlog | grep connection
            //sudo docker exec -it mssqlOnlineSuperMart   "bash"

            ///  /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Strong@Passw0rd"   


            // docker run -p 5050:80 - e 'PGADMIN_DEFAULT_EMAIL=pgadmin4@pgadmin.org' - e 'PGADMIN_DEFAULT_PASSWORD=admin' - d--name pgadmin4 dpage / pgadmin4
            return new OkObjectResult(responseMessage);
        }
    }
}

