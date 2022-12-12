// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//Modified by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN@outlook.com, 20/09/2022 
 

using System.Data.SqlClient;
using System.Linq;
 
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using onlinesupermartSQLElasticDB.Models;
//using Microsoft.ServiceBus;

namespace onlinesupermartSQLElasticDB
{
    public class Sharding
    {
        public ShardMapManager shardMapManager { get; private set; }

        public ListShardMap<int> shardMap { get;  set; }
        public RangeShardMap<int> shardRangeMap { get;  set; }
        public List<string> loggerList { get; set; }
        /// <summary>sss
        /// The shard map manager, or null if it does not exist. 
        /// It is recommended that you keep only one shard map manager instance in
        /// memory per AppDomain so that the mapping cache is not duplicated.
        /// </summary>

        public bool IsNewshard = false;
        public   IConfiguration config = null;
        // Bootstrap Elastic Scale by creating a new shard map manager and a shard map on 
        // the shard map manager database if necessary.
        public Sharding()
        {
            config = SqlDatabaseUtils.GetConfigaration();
            SqlDatabaseUtils.CreateDatabase(config["SQLServerName"], config["ShardMapManagerDatabaseName"], loggerList);

        }

        public ShardMapManager GetShardManager(string smmserver, string smmdatabase, string smmconnstr)
        {
            //config = SqlDatabaseUtils.GetConfigaration();
            // Connection string with administrative credentials for the root database
            SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder(smmconnstr);
            connStrBldr.DataSource = smmserver;
            connStrBldr.InitialCatalog = smmdatabase;
            this.loggerList = loggerList;
            // Deploy shard map manager.
            ShardMapManager smm;
            if (!ShardMapManagerFactory.TryGetSqlShardMapManager(connStrBldr.ConnectionString, ShardMapManagerLoadPolicy.Lazy, out smm))
            {
                this.shardMapManager = ShardMapManagerFactory.CreateSqlShardMapManager(connStrBldr.ConnectionString);
            }
            else
            {
                this.shardMapManager = smm;
            }
            return this.shardMapManager;
        }

        public Sharding(string smmserver, string smmdatabase, string smmconnstr,bool iSRangeShardMap, List<string> loggerList )//Modified
        {
            IsNewshard = false;

            config = SqlDatabaseUtils.GetConfigaration();
            SqlDatabaseUtils.CreateDatabase(config["SQLServerName"], config["ShardMapManagerDatabaseName"], loggerList);
            if (loggerList == null) { loggerList = new List<string>(); }
            GetShardManager(smmserver, smmdatabase, smmconnstr);

            if (!iSRangeShardMap)
            {
                ListShardMap<int> sm;
                if (!shardMapManager.TryGetListShardMap<int>("onlinesupermartSQLElasticDBListShardMap", out sm))
                {
                    this.shardMap = shardMapManager.CreateListShardMap<int>("onlinesupermartSQLElasticDBListShardMap");
                }
                else
                {
                    this.shardMap = sm;
                }
            }
            else
            {
                // Create shard map
                Range<int> range = null;
                int allowedMaxRangePerShard = 5000;
                allowedMaxRangePerShard = int.Parse(config["allowedMaxRangePerShard"]);
                
                shardRangeMap = ShardManagementUtils.CreateOrGetRangeShardMap<int>(this,shardMapManager, config["shardName"], allowedMaxRangePerShard, loggerList);
  
                if (!shardRangeMap.GetShards().Any())//if no shard yet
                {
                    range = new Range<int>(0, allowedMaxRangePerShard);
                    CreateRangeShard.CreateShard(shardRangeMap, range, config["shardName"], loggerList);
                    IsNewshard = true;
                }
                else //check if need to create new shard and get the new range
                {
                    Shard shard = shardRangeMap.GetShards().OrderByDescending(x => x.Location.Database).FirstOrDefault();//only one shard per shardMap;
                    //checked if shard with above range exist
                    //int currentMinHighKey = shardRangeMap.GetMappings().Max(m => m.Value.Low) ;//22/09/2022
                    CreateShardIfNotExists(shardRangeMap,shard, allowedMaxRangePerShard, config["shardName"]);
                    
                }
                 

            }
        }
        public void CreateShardIfNotExists(RangeShardMap<int> shardRangeMap,Shard shard,int allowedMaxRangePerShard, string shardName)
        {
            if (shardRangeMap != null && shard != null)
            {
                //SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder(config["connectionString"]);
                //connStrBldr.DataSource = shard.Location.DataSource;
                //connStrBldr.InitialCatalog = shard.Location.Database;
                int maxRange = 0, minRange = 0;
                SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder(config["connectionString"])
                {
                    UserID = config["sqlAdministratrionUser"],
                    Password = config["sqlAdministratrionPassword"],
                    ApplicationName = config["AppName"]
                };
                connStrBldr.DataSource = shard.Location.DataSource; // s_server;
                connStrBldr.InitialCatalog = shard.Location.Database;

                DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
             new DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
                dbContextOptionsBuilder.UseSqlServer(connStrBldr.ConnectionString);

                // Go into a DbContext to trigger migrations and schema deployment for the new shard.
                // This requires an un-opened connection.
                int cnt = 0;
                //SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
                //{
                //using (var db = new ElasticScaleContext<int>((connStrBldr.ConnectionString)))
               using (var db = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))
                {
                    try
                    { 
                        cnt = db.Blogs.Count();
                    if (cnt > 0)
                        {
                            maxRange = (from b in db.Blogs select b).Max(m => m.BlogId)+1;
                            minRange = (from b in db.Blogs select b).Min(m => m.BlogId);
                        } 
                    }
                    catch (Exception ex) { }
                }
               
                //});
                
                    int currentMaxHighKey = 0;
                if (shardRangeMap.GetMappings().Count > 0)
                {
                    currentMaxHighKey = shardRangeMap.GetMappings().Max(m => m.Value.High)+1;
                    if(maxRange> currentMaxHighKey)
                    {
                        currentMaxHighKey = maxRange;
                    }
                }
                    if (cnt >= allowedMaxRangePerShard) //create new shard//maxRange >= allowedMaxRangePerShard)//create new shard
                    {

                        Range<int> range = new Range<int>(currentMaxHighKey, currentMaxHighKey + allowedMaxRangePerShard);
                        shardName = shardName + range.Low + "";
                        RangeShardMap<int> outshardMap;
                        // if the Shard Map does not exist, so create it
                        if (!shardMapManager.TryGetRangeShardMap<int>(shardName, out outshardMap))
                        {
                            shardRangeMap = shardMapManager.CreateRangeShardMap<int>(shardName);
                            CreateRangeShard.CreateShard(shardRangeMap, range, config["shardName"], loggerList);
                            IsNewshard = true;
                        }

                    }
                
                 
            }
        } 
         
    }
}
