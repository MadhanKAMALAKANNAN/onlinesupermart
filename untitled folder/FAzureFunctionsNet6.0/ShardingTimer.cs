//Created by: Madhan KAMALAKANNAN,  Nov/2022
using System;
using System.Security.Cryptography;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using onlinesupermartSQLElasticDB;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using System.Collections.Generic;
namespace ShardingAZF
{
    public class ShardingTimer
    {
         public ShardingTimer(List<string> loggerList)
        {
            //log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //**********************Creates New ShardMap, Shard if needed*****if its new shard then create Table Schemas and reference keys******************
           /* while(true)
            {
                try
                {
                    if (DateTime.Now.Minute % 2 == 0)//each two minutes
                    {
                        var config = SqlDatabaseUtils.GetConfigaration();
                        Sharding sharding = null; Shard shard = null;
                        ShardingEntryClass shardingEntryClass = new ShardingEntryClass();
                        shardingEntryClass.ShardsFunction(out sharding, out shard, config, loggerList);
                    }
                }
                catch (Exception ex) { }
            }
           */
        }
    }
}

    