//Created by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  Aug/2022
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
namespace onlinesupermartSQLElasticDB
{
    public class ShardingEntryClass
    {
        public ShardingEntryClass()
        {

        }
        public void ShardsFunction(out Sharding sharding, out Shard shard, IConfiguration config, List<string> loggerList)
        {
            //**********************Creates New ShardMap, Shard if needed**********************
            //SqlDatabaseUtils.CreateDatabase(config["SQLServerName"], config["ShardMapManagerDatabaseName"], loggerList);


            sharding = new Sharding(config["SQLServerName"], config["ShardMapManagerDatabaseName"], config["connectionString"], true, loggerList);

            string shardMapeName = config["shardName"];

            sharding.shardRangeMap = ShardManagementUtils.GetLastShardMap(sharding, shardMapeName); //26/09/2022
            shard = sharding.shardRangeMap.GetShards().OrderByDescending(x => x.Location.Database).FirstOrDefault();  
            //***************************if its new shard then create Table Schemas and reference keys**********************************************************

            //SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder
            //{
            //    UserID = config["sqlAdministratrionUser"],
            //    Password = config["sqlAdministratrionPassword"],
            //    ApplicationName = config["AppName"]
            //};
            //connStrBldr.DataSource = shard.Location.DataSource; // s_server;
            //connStrBldr.InitialCatalog = shard.Location.Database;

            //SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            //{
            //    // Go into a DbContext to trigger migrations and schema deployment for the new shard.
            //    // This requires an un-opened connection.
            //    using (var db = new ElasticScaleContext<int>(connStrBldr.ConnectionString))
            //    {
            //        try
            //        {
            //            int cnt = (from b in db.Blogs select b).Count();
            //            if (cnt > 0)
            //            { var maxRange = (from b in db.Blogs select b).Max(m => m.BlogId); }
            //        }
            //        catch (Exception ex) { }
            //    }
            //});
            //**************************************************************************************

        }
    }
}
