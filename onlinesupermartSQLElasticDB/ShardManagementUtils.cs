// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//Modified: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com, //21/09/2022
using System;
using System.Configuration;
using onlinesupermartSQLElasticDB;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using System.Text;
using onlinesupermartSQLElasticDB.Migrations;
using System.Data.SqlClient;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using onlinesupermartSQLElasticDB.Models;
using Microsoft.Extensions.Logging;

namespace onlinesupermartSQLElasticDB
{
    public static class ShardManagementUtils
    {
        /// <summary>
        /// Tries to get the ShardMapManager that is stored in the specified database.
        /// </summary>
        public static ShardMapManager TryGetShardMapManager(string shardMapManagerServerName, string shardMapManagerDatabaseName)
        {
            string shardMapManagerConnectionString = SqlDatabaseUtils.GetConfigaration()["connectionString"];
                      

            if (!SqlDatabaseUtils.DatabaseExists(shardMapManagerServerName, shardMapManagerDatabaseName))
            {
                // Shard Map Manager database has not yet been created
                return null;
            }

            ShardMapManager shardMapManager;
            bool smmExists = ShardMapManagerFactory.TryGetSqlShardMapManager(
                shardMapManagerConnectionString,
                ShardMapManagerLoadPolicy.Lazy,
                out shardMapManager);

            if (!smmExists)
            {
                // Shard Map Manager database exists, but Shard Map Manager has not been created
                return null;
            }

            return shardMapManager;
        }

        /// <summary>
        /// Creates a shard map manager in the database specified by the given connection string.
        /// </summary>
        public static ShardMapManager CreateOrGetShardMapManager(string shardMapManagerConnectionString, List<string> loggerList)
        {
            // Get shard map manager database connection string
            // Try to get a reference to the Shard Map Manager in the Shard Map Manager database. If it doesn't already exist, then create it.
            ShardMapManager shardMapManager;
            bool shardMapManagerExists = ShardMapManagerFactory.TryGetSqlShardMapManager(
                shardMapManagerConnectionString,
                ShardMapManagerLoadPolicy.Lazy,
                out shardMapManager);

            if (shardMapManagerExists)
            {
                loggerList.Add(string.Format("Shard Map Manager already exists"));
            }
            else
            {
                // The Shard Map Manager does not exist, so create it
                shardMapManager = ShardMapManagerFactory.CreateSqlShardMapManager(shardMapManagerConnectionString);
                loggerList.Add(string.Format("Created Shard Map Manager"));
            }

            return shardMapManager;
        }

        /// <summary>
        /// Creates a new Range Shard Map with the specified name, or gets the Range Shard Map if it already exists.
        /// </summary>
        public static RangeShardMap<int> CreateOrGetRangeShardMap<T>(Sharding sharding,ShardMapManager shardMapManager, string shardName,int allowedMaxRangePerShard, List<string> loggerList)
        {
            // Try to get a reference to the Shard Map.
            RangeShardMap<int> shardMap = null;
             
            Range<int> range = new Range<int>(0, allowedMaxRangePerShard);
            string shardName1 = shardName;
            if(shardMapManager.GetShardMaps().Count() == 0)
            {
                shardName = shardName + range.Low + "";
                // The Shard Map does not exist, so create it
                shardMap = shardMapManager.CreateRangeShardMap<int>(shardName);
                loggerList.Add(string.Format("Created Shard Map {0}", shardMap.Name));
            }
            else
            {
                shardMap = ShardManagementUtils.GetLastShardMap(sharding, shardName); //26/09/2022 //(RangeShardMap<int>)shardMapManager.GetShardMaps().OrderByDescending(x => x.Name).FirstOrDefault(); //.TryGetRangeShardMap(shardName, out shardMap);
                shardName = shardMap.Name;
            }

            if (shardMap!=null)
            {
                Shard shard = shardMap.GetShards().OrderByDescending(x => x.Location.Database).FirstOrDefault();//only one shard per shardMap;
                loggerList.Add(string.Format("Shard Map {0} already exists", shardMap.Name));
                sharding.CreateShardIfNotExists(shardMap,shard, allowedMaxRangePerShard, shardName1);
            }
           

            return shardMap;
        }
        public static RangeShardMap<int> GetLastShardMap(Sharding sharding, string shardMapeName)
        {   
            return sharding.shardMapManager.GetRangeShardMap<int>(GetLastShardMapDBName(sharding.shardMapManager, shardMapeName)); 
        }
        public static string GetLastShardMapDBName(ShardMapManager shardMapManager, string shardMapeName)
        {
            var smList = shardMapManager.GetShardMaps();
            if (smList.Count() > 0)
            {
                var LastShard = (from s in smList select int.Parse(s.Name.Replace(shardMapeName, ""))).Max();
                shardMapeName = shardMapeName + LastShard;
            }
            else
            {
                shardMapeName = shardMapeName + 0;
            }
            return shardMapeName;
        }
        /// <summary>
        /// Adds Shards to the Shard Map, or returns them if they have already been added.
        /// </summary>
        public static Shard CreateOrGetShard(ShardMap shardMap, ShardLocation shardLocation, List<string> loggerList) 
        {
            if (loggerList == null) { loggerList = new List<string>(); }
            // Try to get a reference to the Shard
            Shard shard;
            bool shardExists = shardMap.TryGetShard(shardLocation, out shard);

            if (shardExists)
            {
                loggerList.Add(string.Format("Shard {0} has already been added to the Shard Map", shardLocation.Database));
            }
            else
            {
               var  config = SqlDatabaseUtils.GetConfigaration();
                // The Shard Map does not exist, so create it
                shard = shardMap.CreateShard(shardLocation);
                //***************************if its new shard then create Table Schemas and reference keys**********************************************************
                SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder(config["connectionString"])
                {
                    UserID = config["sqlAdministratrionUser"],
                    Password = config["sqlAdministratrionPassword"],
                    ApplicationName = config["AppName"]
                };
                // connStrBldr.DataSource = shard.Location.DataSource; // s_server;
                //connStrBldr.InitialCatalog = shard.Location.Database;
                var shardMapManagerServerName = config["SQLServerName"];
                //SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder(connstr);
                connStrBldr.DataSource = shardMapManagerServerName;
                connStrBldr.InitialCatalog = shard.Location.Database;//databaseName;
                DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
               new DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
                dbContextOptionsBuilder.UseSqlServer(connStrBldr.ConnectionString);
                 int intId = 0;

              var vinte= int.TryParse(shard.Location.Database.Replace("onlinesupermartSQLElasticDBShard", "") + "",out intId) == true ? intId : 1;
                if (intId == 0) { intId = 1; }
                using (var dbCrm = new onlinesupermartSQLElasticDB.Models.AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))//Create Current DBContext Table Schema //15/Oct/2022
                {
                    var migration = new InitialCreate();
                    migration.Up(); // or migration.Down();

                    var prop = migration.GetType().GetProperty("Operations", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (prop != null)
                    {
                        IEnumerable<System.Data.Entity.Migrations.Model.MigrationOperation> operations = prop.GetValue(migration) as IEnumerable<System.Data.Entity.Migrations.Model.MigrationOperation>;
                        var generator = new SqlServerMigrationSqlGenerator();
                        var statements = generator.Generate(operations, "2008");
                        foreach (MigrationStatement item in statements)
                        {
                            try
                            {
                                var tSql = item.Sql.Replace("() ON DELETE CASCADE", " ON DELETE CASCADE");
                                tSql = tSql.Replace("NOT NULL IDENTITY", " NOT NULL IDENTITY(" + intId + ",1) ");

                                dbCrm.Database.ExecuteSqlRaw(tSql);
                            }
                            catch (Exception ex) { }
                        }
                    }
                            // 
                    //dbCrm.RunMigration(migration);
                }
                loggerList.Add(string.Format("Added shard {0} to the Shard Map", shardLocation.Database));
            }

            return shard;
        }
    }
}
