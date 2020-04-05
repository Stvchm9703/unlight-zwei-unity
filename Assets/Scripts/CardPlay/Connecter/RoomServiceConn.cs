using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset.Config;
using ULZAsset.ProtoMod;
using UnityEngine;
public class RoomServiceConn : MonoBehaviour {
    Channel main_ch;
    RoomService.RoomServiceClient client;
    Metadata extra_mt_handle;

    public Room CurrentRoom;
    public RmUserInfo CurrentUser;

    public WSConnect wsConnect;
    List<System.EventHandler<WebSocketSharp.MessageEventArgs>> wscHandler =
        new List<System.EventHandler<WebSocketSharp.MessageEventArgs>>();

    public CfServerSetting config;
    public bool IsHost = false;
    public bool IsWatcher = true;

    void Awake() {
        Debug.Log("on Awake process - Room-Service-Connector");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("room_connector");
        if (objs.Length > 1) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.tag = "room_connector";
        }
    }

    public bool InitSetup(CfServerSetting setting) {
        config = setting;
        // this.main_ch = new Channel(
        //     setting.Host + ":" + setting.Port.ToString() + "/room-service",
        //     ChannelCredentials.Insecure
        // );
        this.main_ch = new Channel(
            setting.Host, setting.Port,
            ChannelCredentials.Insecure
        );
        this.client = new RoomService.RoomServiceClient(this.main_ch);
        this.CurrentUser = new RmUserInfo {
            Id = setting.UserInfo.Id,
            Name = setting.UserInfo.Name,
            Level = setting.UserInfo.Level,
            Rank = setting.UserInfo.Rank,
        };

        return false;
    }

    public async Task<Room> CreateRoom(RoomCreateReq createReq) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        try {
            var create_task = await this.client.CreateRoomAsync(createReq);
            Debug.Log(create_task);
            this.CurrentRoom = create_task;
            this.IsHost = true;
            this.IsWatcher = false;
            return create_task;
        } catch (RpcException) {
            throw;
        }
    }

    public async Task<Room> GetRoom(string room_key, string password, bool isDueler = false) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        try {
            var get_task = await this.client.GetRoomInfoAsync(new RoomReq {
                Key = room_key, Password = password
            });
            CurrentRoom = get_task;
            this.IsHost = false;
            this.IsWatcher = !isDueler;
            return get_task;
        } catch (RpcException) {
            throw;
        }
    }

    public async Task<List<Room>> GetRoomList(RoomCreateReq searchReq) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        try {
            CancellationTokenSource close_tkn = new CancellationTokenSource();
            List<Room> return_list = new List<Room>();
            using(var stream_task = this.client.GetRoomList(searchReq)) {
                // Debug.Log(stream_task);
                while (await stream_task.ResponseStream.MoveNext(close_tkn.Token)) {
                    Debug.Log(stream_task.ResponseStream.Current);
                    return_list.Add(stream_task.ResponseStream.Current);
                }
            }
            return return_list;
        } catch (RpcException) {
            Debug.LogError(this.client);
            Debug.LogError(this.config);
            // Debug.LogError(this.)
            throw;
        }
    }

    public async Task<Room> UpdateRoom(RoomCreateReq updateRoom) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        try {
            var task = await this.client.UpdateRoomAsync(updateRoom);
            return task;
        } catch (RpcException) {
            throw;
        }
    }
    public async Task<RoomMsg> SendMessage(string message) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        if (CurrentRoom == null) {
            throw new System.Exception("NO_CURRENT_ROOM");
        }
        try {
            var msg = new RoomMsg {
                Key = this.CurrentRoom.Key,
                FromId = this.CurrentUser.Id,
                FmName = this.CurrentUser.Name,
                Message = message,
                MsgType = RoomMsg.Types.MsgType.UserText
            };
            var task = await this.client.SendMessageAsync(msg);
            return msg;
        } catch (RpcException) {
            throw;
        }
    }
    public async Task<RoomMsg> SystemSendMessage(string message) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        if (CurrentRoom == null) {
            throw new System.Exception("NO_CURRENT_ROOM");
        }
        try {
            var msg = new RoomMsg {
                Key = this.CurrentRoom.Key,
                FromId = this.CurrentUser.Id,
                FmName = this.CurrentUser.Name,
                Message = message,
                MsgType = RoomMsg.Types.MsgType.SystemInfo,
            };
            var task = await this.client.SendMessageAsync(msg);
            return msg;
        } catch (RpcException) {
            throw;
        }
    }

    public async Task<bool> SendStricker(string stricker_id) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        if (CurrentRoom == null) {
            throw new System.Exception("NO_CURRENT_ROOM");
        }
        try {
            var task = await this.client.SendMessageAsync(
                new RoomMsg {
                    Key = this.CurrentRoom.Key,
                        FromId = this.CurrentUser.Id,
                        FmName = this.CurrentUser.Name,
                        Message = stricker_id,
                        MsgType = RoomMsg.Types.MsgType.UserStricker,
                }
            );
            return true;
        } catch (RpcException) {
            throw;
        }
    }

    public async Task<Room> JoinRoom(string roomKey, string password) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        if (CurrentRoom != null) {
            throw new System.Exception("CURRENT_ROOM_EXIST");
        }
        try {
            var get_task = await this.client.JoinRoomAsync(new RoomReq {
                Key = roomKey,
                    Password = password,
                    User = CurrentUser
            });
            CurrentRoom = get_task;
            this.IsHost = false;
            this.IsWatcher = false;
            return get_task;
        } catch (RpcException) {
            throw;
        }
    }
    public async Task<bool> QuitRoom() {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        if (CurrentRoom != null) {
            throw new System.Exception("CURRENT_ROOM_EXIST");
        }
        try {
            await this.DisconnectToBroadcast();
            var get_task = await this.client.QuitRoomAsync(new RoomReq {
                Key = CurrentRoom.Key,
                    User = CurrentUser
            });
            CurrentRoom = null;
            return true;
        } catch (RpcException) {
            throw;
        }
    }
    public async Task<bool> ChangeCharCard(int charcard_id, int cardset_id, int level) {
        if (main_ch == null || client == null) {
            throw new System.Exception("CONNECT_CLIENT_IS_NULL");
        }
        if (CurrentRoom != null) {
            throw new System.Exception("CURRENT_ROOM_EXIST");
        }
        try {
            await this.client.UpdateCardAsync(new RoomUpdateCardReq {
                Side = this.IsHost ? 0 : 1,
                    Key = CurrentRoom.Key,
                    CharcardId = charcard_id,
                    CardsetId = cardset_id,
                    Level = level
            });
            return true;
        } catch (RpcException) {
            throw;
        }
    }

    /// <summary>
    /// ConnectToBroadcast : the Broadcast handler
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ConnectToBroadcast() {
        Debug.Log("try connect to Broacast");
        if (this.wsConnect == null) {
            this.wsConnect = new WSConnect();
        }
        var conn = await this.wsConnect.ConnectToBroadcast(
            this.CurrentRoom.Key, this.config, wscHandler);
        if (!conn) {
            return false;
        }
        // await this.conn.RoomBroadcast.EmitAsync("join_room", this.current_room.Key);
        return true;
    }

    /// <summary>
    /// AddPendingEventFunc : 
    ///     to add mounting function in WebSocketBroadcast.OnMessage 
    /// </summary>
    /// <param name="funcHandler">
    ///     the event handler that mount with the MessageEventArg
    ///     P.S.: Add the event-handler before the WebSocket Connect (ConnectToBroadcast)
    /// </param>
    /// <returns></returns>
    public bool AddPendingEventFunc(System.EventHandler<WebSocketSharp.MessageEventArgs> funcHandler) {
        this.wscHandler.Add(funcHandler);
        return true;
    }

    /// <summary>
    /// AddPendingEventFunc : 
    ///     to add mounting function in WebSocketBroadcast.OnMessage
    /// </summary>
    /// <param name="funcHandler">
    /// the event handler that mount with the RoomMsg 
    ///     Note: this method have the wrapped function to Parse the message to 
    ///         predefined Msg ( for ULZ-RoomService Connector )
    /// </param>
    /// <returns></returns>
    public bool AddPendingEventFunc(System.EventHandler<RoomMsg> funcHandler) {
        var wrapFunc = new System.EventHandler<WebSocketSharp.MessageEventArgs>((co, msg) => {
            Debug.Log($"sender:{co.ToString()}");
            Debug.Log($"Msg :{msg.Type.ToString()}");
            var msgBlock = RoomMsg.Parser.ParseFrom(
                msg.RawData
            );
            funcHandler(co, msgBlock);
        });
        this.wscHandler.Add(wrapFunc);
        return true;
    }

    /// <summary>
    /// ClearPendingEventFunc(): 
    ///     to clear up the current mounted function in OnMessage event-handler  
    ///     
    /// </summary>
    /// <returns></returns>
    public bool ClearPendingEventFunc() {
        foreach (var trig in this.wscHandler) {
            Debug.Log("try remove func");
            this.wsConnect.ClearEventFunc(trig);
        }
        return true;
    }
    public async Task<bool> DisconnectToBroadcast() {
        this.ClearPendingEventFunc();
        return await this.wsConnect.DisconnectToBroadcast();
    }

    public async Task<bool> Kill() {
        // if (this.ChatRoomStream != null) {

        //     this.CloseChatRoomToken.Cancel();
        //     this.ChatRoomStream = null;
        // }
        await this.DisconnectToBroadcast();
        if (this.client != null) {
            this.client = null;
            await main_ch.ShutdownAsync();
        }
        Destroy(this.gameObject, 0.5f);
        return true;
    }
}