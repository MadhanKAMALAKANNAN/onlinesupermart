using System;
using System.Security.Cryptography;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using onlinesupermartSQLElasticDB;

namespace ShardingAZF
{
    public class ShardingTimer
    {
        [FunctionName("ShardingTimer")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //**********************Creates New ShardMap, Shard if needed*****if its new shard then create Table Schemas and reference keys******************

            Sharding sharding = null; Shard shard = null;
            ShardingEntryClass shardingEntryClass = new ShardingEntryClass();
            shardingEntryClass.ShardsFunction(out sharding, out shard, config);
        }
    }
}

