using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ULZAsset.Config;
using UnityEngine;
using WebSocketSharp;
namespace ULZAsset {
    public class WSConnect {
        public WebSocket RoomCast;
        //  --------------------------------------------
        // Socket-IO
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        public async Task<bool> ConnectToBroadcast(string RoomKey, CfServerSetting conf = null) {
            var wsclient = new WebSocket($"ws://{conf.RoomService.Host}:8000/{RoomKey}");
            wsclient.OnOpen += (type, e) => {
                Debug.Log($"Reconnection happened, type: {type}, url: {wsclient.Url}");
            };
            wsclient.OnError += (info, e) => {
                Debug.LogWarning(info.ToString());
                Debug.LogWarning(e);
            };

            wsclient.OnMessage += (msg, e) => {
                Debug.Log($"Message received: {e}");
                Debug.Log($"Message : {e.RawData.ToString()}");
            };

            wsclient.ConnectAsync();
            this.RoomCast = wsclient;
            // ExitEvent.WaitOne();
            this.RoomCast.Ping();
            return true;

        }

        public async Task<bool> ConnectToBroadcast(
            string RoomKey,
            CfServiceClientSetting conf = null,
            List<EventHandler<MessageEventArgs>> MsgHandler = null
        ) {
            var wsclient = new WebSocket($"ws://{conf.Host}:11020/{RoomKey}");
            wsclient.OnOpen += (type, e) => {
                Debug.Log($"Connected url: {wsclient.Url}");
            };
            wsclient.OnError += (info, e) => {
                Debug.LogWarning(info.ToString());
                Debug.LogWarning(e.Message);
            };

            foreach (var func in MsgHandler) {
                wsclient.OnMessage += func;
            }
            wsclient.ConnectAsync();
            this.RoomCast = wsclient;
            // ExitEvent.WaitOne();
            this.RoomCast.Ping();
            return true;

        }
        public bool AddEventFunc(System.EventHandler<WebSocketSharp.MessageEventArgs> func) {
            if (this.RoomCast != null && this.RoomCast.IsAlive) {
                this.RoomCast.OnMessage += (func);
                return true;
            }
            return false;
        }
        public bool ClearEventFunc(System.EventHandler<WebSocketSharp.MessageEventArgs> func) {
            try {
                this.RoomCast.OnMessage -= (func);
            } catch (SystemException e) {
                Debug.LogWarning(e);
                return false;
            }
            return true;
        }
        public async Task<bool> DisconnectToBroadcast() {
            if (this.RoomCast != null) {
                this.RoomCast.Close(CloseStatusCode.Normal);
            }
            Debug.Log("disconnected");
            return true;
        }
    }
}