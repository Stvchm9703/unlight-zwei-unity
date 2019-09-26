using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using YamlDotNet;
namespace UnlightCli {

    public static class ResxConf {
        public static string info_coll_name { get { return "resx_info"; } }
        public static string info_log_file{ get { return global;}}
    }
    public interface IResxMod {
        string HostAddr {
            get;
        }

        // / <summary>
        // /     Local-File-Address : Local file path for storing file
        // / </summary>
        // / <value></value>
        string LocalFileAddr {
            get;
        }

        // / <summary>
        // /     ObjId : Remote-Local checksum ID,
        // /             to make sure the local resource is same to remote
        // / </summary>
        // / <value></value>
        string ObjId {
            get;
        }
        string KeyId {
            get;
        }

        // / <summary>
        // /     version : Remote-Local checksum ID,
        // /             to make sure the local resource is same to remote
        // / </summary>
        // / <value></value>
        string Version {
            get;
        }

        DateTime lastUpdate {
            get;
        }

        // /-------------------------------------------------------------------------------------
        // / <summary>
        // /     Check Remote file is same version to current object, or not
        // / </summary>
        bool CheckRemote(MongoClient mgo_cli);

        // /-------------------------------------------------------------------------------------
        // / Readd Comp part
        // /
        // / <summary>
        // /     Get Resource Data :
        // /         - Fetch Remote : Get the data from remote to local File-IO
        // / </summary>
        bool FetchRemote(MongoClient mgo_cli);

        // / <summary>
        // /     Get Resource Data :
        // /         - LoadLocal : Get the data local File-IO to Unity Resource
        // / </summary>
        bool LoadLocal();

        // /-------------------------------------------------------------------------------------
        // / Update Comp part
        // / <summary>
        // /     Update Remote Data:
        // /         - Update Remote : Upload local data to remote resource
        // / </summary>
        bool UpdateRemote(MongoClient mgo_cli);

        // / <summary>
        // /     Update Local Data:
        // /         - Update Local : Download remote resource data to local file
        // / </summary>
        bool UpdateLocal();

        // /-------------------------------------------------------------------------------------
        // / Create Comp part
        // / <summary>
        // /     Create Remote Data:
        // /         - Create Remote : Upload local data to remote resource,
        // /             when the remote resource database not exist this object
        // /     !warn: It may not implete / allow to use for end-player-user !
        // / </summary>
        bool CreateRemote(MongoClient mgo_cli);

        // / <summary>
        // /     Create Remote Data:
        // /         - Create Local : Upload local data from remote resource,
        // /             when game-prog initialize, or find missnig files, please use it
        // / </summary>
        bool CreateLocal();

        // /-------------------------------------------------------------------------------------
        // / Delete Comp part
        // / <summary>
        // /     Delete Remote Data:
        // /         - Delete Remote : remove remote resource,
        // /         it must add log for mongodb monitor
        // /     !warn: It may not implete / allow to use for end-player-user !
        // / </summary>
        bool DeleteRemote(MongoClient mgo_cli);

        // / <summary>
        // /     Delete Local Data:
        // /         - Delete Local : remove local data
        // /             when the end-player-user remove for application
        // / </summary>
        bool DeleteLocal();

    }

    protected class ResxInfoMod {
        public string mod_name;
        public DateTime last_update;
        public string version;
        public string last_modified;
    }
    public class ResxMod: IResxMod {
        string local_file_addr;
        string remote_resx_addr;
        CfLocalSetting local_setting;
        CfServerSetting remote_setting;
        string remote_svr_type; // default : mongodb
        string file_type;
        string log_addr;
        string key_id;
        string o_id;
        string version;
        DateTime last_update;

        public string HostAddr {
            get {
                return this.local_file_addr;
            }
        }

        public string LocalFileAddr {
            get {
                return this.local_file_addr;
            }
        }

        public string ObjId {
            get {
                return this.o_id;
            }
        }

        public string KeyId {
            get {
                return this.key_id;
            }
        }

        public string Version {
            get {
                return this.version;
            }
        }

        public DateTime lastUpdate {
            get {
                return this.last_update;
            }
        }

        public ResxMod(ConfigTemp conf) {}

        public bool CheckRemote(MongoClient mgo_cli) {
            if (mgo_cli == null) {
                Debug.Log("mongodb client is not inited");
                return false;
            }
            
            // var info_blck = 
            string type_name = this.GetType().ToString();
            var db = mgo_cli.GetDatabase(this.remote_setting.Database);
            var coll = db.GetCollection<ResxInfoMod>( UnlightCli.ResxConf.info_coll_name );
            
            return true;
        }
        public bool FetchRemote(MongoClient mgo_cli) {
            return true;
        }

        public bool LoadLocal() {
            return true;
        }

        public bool UpdateRemote(MongoClient mgo_cli) {
            return true;
        }

        public bool UpdateLocal() {
            return true;
        }

        public bool CreateRemote(MongoClient mgo_cli) {
            return true;
        }

        public bool CreateLocal() {
            return true;
        }

        public bool DeleteRemote(MongoClient mgo_cli) {
            return true;
        }

        public bool DeleteLocal() {
            return true;
        }
    }

}
