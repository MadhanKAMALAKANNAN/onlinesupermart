using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace testtimer
{
    public class testtimer
    {
        [FunctionName("testtimer")]
        public void Run([TimerTrigger("0 30 9 * * *",
           //  #if DEBUG
           RunOnStartup = true
           //#endif
            )
            ]TimerInfo myTimer, ILogger log)
        {
          

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
    
    