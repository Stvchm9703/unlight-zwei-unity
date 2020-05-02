using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ULZAsset.Config {
    public static class ConfigPath {
        public static string StreamingAsset {
            get {
#if UNITY_EDITOR || UNITY_STANDALONE	
                return Application.streamingAssetsPath;
#elif UNITY_ANDROID || UNITY_IOS 
                return Path.Combine(Application.persistentDataPath, "streamingAsset");
#endif
            }
        }
        public static string Asset_path {
            get {
                var tmp = "";
                switch (Application.platform) {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                        tmp = Path.Combine("win", "x86");
                        break;
                    case RuntimePlatform.Android:
                        tmp = "android";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        tmp = "ios";
                        break;
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                        tmp = "mac";
                        break;
                }
                return Path.Combine(Application.streamingAssetsPath, tmp);
            }
        }
    }

    [System.Serializable]
    public class ConfigTempContainer {
        public string work_dir_path;
        public CfServerSetting remote;
        public List<int> card_set;
    }

    [System.Serializable]
    public class CfServerSetting {
        public CfServiceClientSetting RoomService;
        public CfServiceClientSetting CredService;
        public CfServiceClientSetting GameDuelService;
        public CfUserInfo UserInfo;
    }

    [System.Serializable]
    public class CfServiceClientSetting {
        public string ServiceName;
        public string Connector;
        public string Host;
        public int Port;
        public string Database;
        public string Username;
        public string Password;
        public string Key;
        public string KeyPemPath;
        public CfStreamSetting StreamSetting;
    }

    [System.Serializable]
    public class CfStreamSetting {
        public string Connector;
        public string Host;
        public int Port;
        public string KeyPemPath;

    }

    [System.Serializable]
    public class CfCardVersion {
        public string Version;
        public List<int> Available;
        public string LastUpdate;
    }

    [System.Serializable]
    public class CfUserInfo {
        public string Id;
        public string Name;
        public string Title;
        public int Level;
        public int Rank;
    }

    public static class ConfigContainer {
        public static ConfigTempContainer LoadCfFile(string work_dir) {
            var t = new ConfigTempContainer();
            t.work_dir_path = work_dir;

            // load config file
            string[] tpath = { work_dir, "config.yaml" };
            var r = Path.Combine(tpath);
            Debug.Log(r);
            if (File.Exists(r)) {
                try {
                    using(StreamReader reader = new StreamReader(r)) {
                        var deserializer = new DeserializerBuilder()
                            .Build();
                        t.remote = deserializer.Deserialize<CfServerSetting>(reader);
                    }
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }

            return t;

        }

        public static CfCardVersion LoadCardVersion(string work_dir) {
            // load character card update file
            string[] cpath = { work_dir, "card_set_update.log.yml" };
            var r = Path.Combine(cpath);
            Debug.Log(r);
            var t = new CfCardVersion();
            if (File.Exists(r)) {
                try {
                    using(StreamReader reader = new StreamReader(r)) {
                        var deserializer = new DeserializerBuilder()
                            .Build();
                        t = deserializer.Deserialize<CfCardVersion>(reader);
                    }
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }
            return t;
        }

        public static async Task<bool> CreateCfFile(string out_dir, CfServerSetting setting) {
            var serializer = new SerializerBuilder().Build();
            var yml = serializer.Serialize(setting);
            string[] tpath = { out_dir, "config.yaml" };

            using(var sw = new StreamWriter(Path.Combine(tpath))) {
                await sw.WriteAsync(yml);
            }
            return true;
        }

    }
}