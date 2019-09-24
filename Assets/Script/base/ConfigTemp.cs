using System;
using System.IO;
using UnityEngine;
// using UnityEngine.JsonUnity;
using Newtonsoft.Json;
using YamlDotNet.RepresentationModel;
// using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnlightCli {
    public class ConfigTempContainer {
        public string work_dir_path;
        public ConfigTemp content;
    }
    public class ConfigTemp {
        public string root_path;

        public CfModuleSetting Log;
        public CfModuleSetting Resource;
        public CfModuleSetting Duel;
    }
    public class CfModuleSetting {
        public CfServerSetting Remote;
        public CfLocalSetting Local;
    }

    public class CfServerSetting {
        public string Connector;
        public string Host;
        public int Port;
        public string Database;
        public string Username;
        public string Password;
        public string Key;
    }

    public class CfLocalSetting {
        public string Format;
        public string Schema;
        public string Filepath;

    }

    public static class Config {
        public static ConfigTempContainer LoadCfFile (string work_dir) {
            var t = new ConfigTempContainer ();
            t.work_dir_path = work_dir;
            string[] tpath = {
                work_dir,
                "config.yaml"
            };
            var r = Path.Combine (tpath);
            if (File.Exists (r)) {
                try {
                    using (var reader = new StreamReader (work_dir)) {
                        var yaml = new YamlStream ();
                        yaml.Load (reader);
                        Debug.Log (yaml);
                        var mapping = (YamlMappingNode) yaml.Documents[0].RootNode;
                    }
                } catch (Exception e) {
                    Debug.Log (e);
                }
                // var input = new StringReader (File.ReadAllText (work_dir));

            }
            return t;

        }

        public static bool CreateCfFile (string out_dir, ConfigTemp setting) {

            var serializer = new SerializerBuilder ().Build ();
            // var lin = Helper;
            string[] tpath = {
                out_dir,
                "config.yaml"
            };
            if (!File.Exists (Path.Combine (tpath))) {
                File.CreateText (Path.Combine (tpath));
            }
            var obj = JsonConvert.SerializeObject (setting);
            Debug.Log (obj);
            string yml = serializer.Serialize (setting);
            File.AppendAllText (Path.Combine (tpath), yml);
            return true;
        }
    }
}