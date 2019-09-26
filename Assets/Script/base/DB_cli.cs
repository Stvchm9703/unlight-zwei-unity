using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

using UnlightCli;
namespace UnlightCli {
    public static class DB {
        public static MongoClient MgoConnect(CfServerSetting config) {
            string config_conn = config.Connector + "://" + config.Host + ":" + config.Port;
            if (!string.IsNullOrEmpty(config.Username) || 
                !string.IsNullOrEmpty(config.Password)) {
                config_conn = 
                    config.Connector + "://" + 
                    config.Username + ":" + config.Password + "@"+ 
                    config.Host + ":" + config.Port;
            }
            return new MongoClient(config_conn);
        }

        public static RedisClient RedisConnect(){

        }
    }

}
