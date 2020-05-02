using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using ULZAsset;
using ULZAsset.Config;
using ULZAsset.ProtoMod;
using UnityEngine;
public class AC_CertConn : MonoBehaviour {
    // public CfServerSetting ConfigForm;
    public CfServiceClientSetting ConfigForm;

    private Channel auth_chan, main_room_chan;
    private RoomService.RoomServiceClient test_cli;
    private CreditsAuth.CreditsAuthClient create_auth_cli;
    private string file;

    public bool TryConnectAuthServ(string ip_address, int port) {
        try {
            this.auth_chan = new Channel(
                ip_address, port, ChannelCredentials.Insecure
            );
            this.create_auth_cli = new CreditsAuth.CreditsAuthClient(this.auth_chan);

            ConfigForm.Connector = "grpc";
            ConfigForm.Host = ip_address;
            ConfigForm.Port = 11000;
            return true;
        } catch (RpcException e) {
            Debug.LogError(e);
            return false;
            throw;
        }
    }

    public async Task<bool> CreateAccount(string username, string password) {
        if (this.create_auth_cli == null) {
            return false;
        }

        try {
            var req = new CredReq {
                Ip = Dns.GetHostName(),
                Username = username,
                Password = password,
            };
            CheckCredResp result = await this.create_auth_cli.CreateCredAsync(req);
            ConfigForm.Username = username;
            ConfigForm.Password = password;

            return true;

        } catch (RpcException e) {
            Debug.LogError(e);
            return false;
            throw;
        }
    }
    public async Task<bool> TryLogin(string username, string password) {
        if (this.create_auth_cli == null) {
            return false;
        }
        try {
            var req = new CredReq {
                Ip = Dns.GetHostName(),
                Username = username,
                Password = password,
            };
            CheckCredResp result = await this.create_auth_cli.CheckCredAsync(req);
    
            ConfigForm.Username = username;
            ConfigForm.Password = password;

            // 
            return true;
        } catch (RpcException e) {
            Debug.LogError(e);
            return false;
            throw;
        }
    }

    public async Task<bool> GetPemFile() {
        if (this.create_auth_cli == null) {
            return false;
        }
        try {
            var req = new CredReq {
                Ip = Dns.GetHostName(),
                Username = ConfigForm.Username,
                Password = ConfigForm.Password,
            };
            var close_tkn = new CancellationTokenSource();
            string[] tpath = { ConfigPath.StreamingAsset, "key.pem" };

            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(tpath)))) {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(
                        Path.Combine(tpath)));
            }

            StreamWriter sw = (File.Exists(Path.Combine(tpath))) ?
                File.AppendText(Path.Combine(tpath)) :
                File.CreateText(Path.Combine(tpath));

            using(var result = this.create_auth_cli.GetCred(req)) {
                while (await result.ResponseStream.MoveNext(close_tkn.Token)) {
                    var resp_tmp = result.ResponseStream.Current;
                    sw.WriteLine(resp_tmp.File.ToStringUtf8());
                    file = file + resp_tmp.File.ToStringUtf8() + "\n";
                }
            }
            sw.Close();
            Debug.Log(file);
            // try resolve
            var crt = new SslCredentials(Path.Combine(tpath));
            // no error 

            // file = File.ReadAllText(Path.Combine(tpath));
            ConfigForm.KeyPemPath = "%StreamAsset%/" + "key.pem";
            // File.Delete(Path.Combine(tpath));
            return true;
        } catch (RpcException e) {
            Debug.LogError(e);
            return false;
            throw;
        }
    }

    public async Task<bool> TryConnectMain(string ip_address, int port) {
        try {

            var crt = new SslCredentials(file);
            this.main_room_chan = new Channel(
                ip_address, port, crt
            );

            if (ConfigForm.Connector == "") {
                ConfigForm.Connector = "grpc";
            }
            if (ConfigForm.Host == "") {
                ConfigForm.Host = ip_address;
            }
            if (ConfigForm.Port == 0) {
                ConfigForm.Port = port;
            }
            return true;

        } catch (RpcException e) {
            Debug.LogError(e);
            return false;
            throw e;
        }

    }

    public async Task<bool> SaveAsset() {
        ConfigForm.KeyPemPath = "%StreamAsset%/" + "key.pem";
        var cfForm = new CfServerSetting();
        cfForm.CredService = ConfigForm;
        ConfigContainer.CreateCfFile(ConfigPath.StreamingAsset, cfForm);
        string[] tpath = { ConfigPath.StreamingAsset, "key.pem" };

        File.WriteAllText(
            Path.Combine(tpath),
            file);

        return true;
    }

    void Destory() {
        if (test_cli != null) {
            this.test_cli = null;
            this.main_room_chan.ShutdownAsync().Wait();
        }
        if (create_auth_cli != null) {
            this.create_auth_cli = null;
            this.auth_chan.ShutdownAsync().Wait();
        }

    }
}