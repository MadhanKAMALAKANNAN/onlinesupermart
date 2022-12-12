//Created by:Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  Nov/2022

using System;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using onlinesupermartSQLElasticDB;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace ShardingSimulationTimer
{
    
    public class ShardingSimulationTimerClass
    {
        string shardingName = "";
        public bool NoNewSharding = false;

        public ShardingSimulationTimerClass()      {
           ShardingSimulationTimerMethod(null, null);
             System.Timers.Timer  timer = new System.Timers.Timer(60* 1000 * 1);
            //timer.
             timer.Enabled = true;
             timer.Elapsed +=  new System.Timers.ElapsedEventHandler(ShardingSimulationTimerMethod);
            // Have the timer fire repeated events (true is the default)
         timer.AutoReset = true;

            Console.WriteLine("ShardingSimulationTimer Started!");


        }

        public void ShardingSimulationTimerMethod(Object sender, ElapsedEventArgs e)
            {
            // See https://aka.ms/new-console-template for more information

            Console.WriteLine("ShardingSimulationTimer runs {0}!",DateTime.Now);

            //while (true)
            {
                try
                {
                    //if (DateTime.Now.Minute % 2 == 0)//each two minutes
                   {
                        
                        List<string> loggerList = new List<string>();
                        var config = SqlDatabaseUtils.GetConfigaration();
                        Sharding sharding = null; Shard shard = null;
                        ShardingEntryClass shardingEntryClass = new ShardingEntryClass();
                        shardingEntryClass.ShardsFunction(out sharding, out shard, config, loggerList);

                        if(sharding.IsNewshard)
                        { 
                            shardingName = shard.Location.Database;
                            Console.WriteLine("new sharding got created : {0}", shardingName);
                            NoNewSharding = false;
                        }
                        else
                        {
                            NoNewSharding = true;
                            Console.WriteLine("No new sharding");
                        }
                         

                    }
                }
                catch (Exception ex) { }
            }
            Console.WriteLine("ShardingSimulationTimer finished {0}!", DateTime.Now);

        }
    }
}

