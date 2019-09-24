using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnlightCli;
public class CardDuelLogic : MonoBehaviour {
    // Start is called before the first frame update
    void Awake () {
        var wd = Directory.GetCurrentDirectory ();
        Debug.Log (wd);
        // var ert = UnlightCli.Config.LoadCfFile (wd);
        // if (!ert) {
        var t = new UnlightCli.ConfigTemp {
            root_path = wd,
            Log = new UnlightCli.CfModuleSetting {
                Remote = new UnlightCli.CfServerSetting{
                    Connector = "mysql",
                    Host = "127.0.0.1",
                    Port = 3306,
                    Database = "log",
                    Username = "root",
                    Password = "",
                    Key = "",
                },
                Local = new UnlightCli.CfLocalSetting {
                    Format = "",
                    Schema = "sql",
                    Filepath = wd + "/data/log.db",
                },
            },
            Resource = new UnlightCli.CfModuleSetting {
                Remote = new UnlightCli.CfServerSetting{
                    Connector = "mongodb",
                    Host = "127.0.0.1",
                    Port = 27017,
                    Database = "unlight_resx",
                    Username = "root",
                    Password = "",
                    Key = "",
                },
                Local = new UnlightCli.CfLocalSetting {
                    Format = "",
                    Schema = "sql",
                    Filepath = wd + "/data/resx",
                },
            },
            Duel = new UnlightCli.CfModuleSetting {
                Remote = new UnlightCli.CfServerSetting{
                    Connector = "redis",
                    Host = "127.0.0.1",
                    Port = 27017,
                    Database = "unlight_duel",
                    Username = "root",
                    Password = "",
                    Key = "",
                },
                Local = new UnlightCli.CfLocalSetting {
                    Format = "",
                    Schema = "sql",
                    Filepath = wd + "/data/resx",
                },
            },
        };
        UnlightCli.Config.CreateCfFile (wd, t);
        // }
    }
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
}

// public class CardDuelGameLogic : MonoBehaviour {

//     void Start () {
//         string msg = "<b>on start</b>";
//         Debug.Log (msg);
//     }
// }