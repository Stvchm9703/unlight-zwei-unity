// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: RoomService/service.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace ULZAsset.ProtoMod {
  public static partial class RoomService
  {
    static readonly string __ServiceName = "ULZProto.RoomService";

    static readonly grpc::Marshaller<global::ULZAsset.ProtoMod.RoomCreateReq> __Marshaller_ULZProto_RoomCreateReq = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ULZAsset.ProtoMod.RoomCreateReq.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::ULZAsset.ProtoMod.Room> __Marshaller_ULZProto_Room = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ULZAsset.ProtoMod.Room.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::ULZAsset.ProtoMod.RoomReq> __Marshaller_ULZProto_RoomReq = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ULZAsset.ProtoMod.RoomReq.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::ULZAsset.ProtoMod.RoomMsg> __Marshaller_ULZProto_RoomMsg = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ULZAsset.ProtoMod.RoomMsg.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::ULZAsset.ProtoMod.Empty> __Marshaller_ULZProto_Empty = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ULZAsset.ProtoMod.Empty.Parser.ParseFrom);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room> __Method_CreateRoom = new grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CreateRoom",
        __Marshaller_ULZProto_RoomCreateReq,
        __Marshaller_ULZProto_Room);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room> __Method_GetRoomList = new grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "GetRoomList",
        __Marshaller_ULZProto_RoomCreateReq,
        __Marshaller_ULZProto_Room);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Room> __Method_GetRoomInfo = new grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Room>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetRoomInfo",
        __Marshaller_ULZProto_RoomReq,
        __Marshaller_ULZProto_Room);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room> __Method_UpdateRoom = new grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(
        grpc::MethodType.Unary,
        __ServiceName,
        "UpdateRoom",
        __Marshaller_ULZProto_RoomCreateReq,
        __Marshaller_ULZProto_Room);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.RoomMsg> __Method_ServerBroadcast = new grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.RoomMsg>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "ServerBroadcast",
        __Marshaller_ULZProto_RoomReq,
        __Marshaller_ULZProto_RoomMsg);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomMsg, global::ULZAsset.ProtoMod.Empty> __Method_SendMessage = new grpc::Method<global::ULZAsset.ProtoMod.RoomMsg, global::ULZAsset.ProtoMod.Empty>(
        grpc::MethodType.Unary,
        __ServiceName,
        "SendMessage",
        __Marshaller_ULZProto_RoomMsg,
        __Marshaller_ULZProto_Empty);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Empty> __Method_QuitRoom = new grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Empty>(
        grpc::MethodType.Unary,
        __ServiceName,
        "QuitRoom",
        __Marshaller_ULZProto_RoomReq,
        __Marshaller_ULZProto_Empty);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room> __Method_QuickPair = new grpc::Method<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(
        grpc::MethodType.Unary,
        __ServiceName,
        "QuickPair",
        __Marshaller_ULZProto_RoomCreateReq,
        __Marshaller_ULZProto_Room);

    static readonly grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Room> __Method_JoinRoom = new grpc::Method<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Room>(
        grpc::MethodType.Unary,
        __ServiceName,
        "JoinRoom",
        __Marshaller_ULZProto_RoomReq,
        __Marshaller_ULZProto_Room);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::ULZAsset.ProtoMod.ServiceReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of RoomService</summary>
    [grpc::BindServiceMethod(typeof(RoomService), "BindService")]
    public abstract partial class RoomServiceBase
    {
      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Room> CreateRoom(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task GetRoomList(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::IServerStreamWriter<global::ULZAsset.ProtoMod.Room> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Room> GetRoomInfo(global::ULZAsset.ProtoMod.RoomReq request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Room> UpdateRoom(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task ServerBroadcast(global::ULZAsset.ProtoMod.RoomReq request, grpc::IServerStreamWriter<global::ULZAsset.ProtoMod.RoomMsg> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Empty> SendMessage(global::ULZAsset.ProtoMod.RoomMsg request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Empty> QuitRoom(global::ULZAsset.ProtoMod.RoomReq request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Room> QuickPair(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ULZAsset.ProtoMod.Room> JoinRoom(global::ULZAsset.ProtoMod.RoomReq request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for RoomService</summary>
    public partial class RoomServiceClient : grpc::ClientBase<RoomServiceClient>
    {
      /// <summary>Creates a new client for RoomService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public RoomServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for RoomService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public RoomServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected RoomServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected RoomServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::ULZAsset.ProtoMod.Room CreateRoom(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return CreateRoom(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Room CreateRoom(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_CreateRoom, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> CreateRoomAsync(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return CreateRoomAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> CreateRoomAsync(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_CreateRoom, null, options, request);
      }
      public virtual grpc::AsyncServerStreamingCall<global::ULZAsset.ProtoMod.Room> GetRoomList(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetRoomList(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncServerStreamingCall<global::ULZAsset.ProtoMod.Room> GetRoomList(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_GetRoomList, null, options, request);
      }
      public virtual global::ULZAsset.ProtoMod.Room GetRoomInfo(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetRoomInfo(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Room GetRoomInfo(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetRoomInfo, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> GetRoomInfoAsync(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetRoomInfoAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> GetRoomInfoAsync(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetRoomInfo, null, options, request);
      }
      public virtual global::ULZAsset.ProtoMod.Room UpdateRoom(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return UpdateRoom(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Room UpdateRoom(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_UpdateRoom, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> UpdateRoomAsync(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return UpdateRoomAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> UpdateRoomAsync(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_UpdateRoom, null, options, request);
      }
      public virtual grpc::AsyncServerStreamingCall<global::ULZAsset.ProtoMod.RoomMsg> ServerBroadcast(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ServerBroadcast(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncServerStreamingCall<global::ULZAsset.ProtoMod.RoomMsg> ServerBroadcast(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_ServerBroadcast, null, options, request);
      }
      public virtual global::ULZAsset.ProtoMod.Empty SendMessage(global::ULZAsset.ProtoMod.RoomMsg request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return SendMessage(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Empty SendMessage(global::ULZAsset.ProtoMod.RoomMsg request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_SendMessage, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Empty> SendMessageAsync(global::ULZAsset.ProtoMod.RoomMsg request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return SendMessageAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Empty> SendMessageAsync(global::ULZAsset.ProtoMod.RoomMsg request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_SendMessage, null, options, request);
      }
      public virtual global::ULZAsset.ProtoMod.Empty QuitRoom(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return QuitRoom(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Empty QuitRoom(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_QuitRoom, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Empty> QuitRoomAsync(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return QuitRoomAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Empty> QuitRoomAsync(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_QuitRoom, null, options, request);
      }
      public virtual global::ULZAsset.ProtoMod.Room QuickPair(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return QuickPair(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Room QuickPair(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_QuickPair, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> QuickPairAsync(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return QuickPairAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> QuickPairAsync(global::ULZAsset.ProtoMod.RoomCreateReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_QuickPair, null, options, request);
      }
      public virtual global::ULZAsset.ProtoMod.Room JoinRoom(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return JoinRoom(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ULZAsset.ProtoMod.Room JoinRoom(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_JoinRoom, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> JoinRoomAsync(global::ULZAsset.ProtoMod.RoomReq request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return JoinRoomAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ULZAsset.ProtoMod.Room> JoinRoomAsync(global::ULZAsset.ProtoMod.RoomReq request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_JoinRoom, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override RoomServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new RoomServiceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(RoomServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_CreateRoom, serviceImpl.CreateRoom)
          .AddMethod(__Method_GetRoomList, serviceImpl.GetRoomList)
          .AddMethod(__Method_GetRoomInfo, serviceImpl.GetRoomInfo)
          .AddMethod(__Method_UpdateRoom, serviceImpl.UpdateRoom)
          .AddMethod(__Method_ServerBroadcast, serviceImpl.ServerBroadcast)
          .AddMethod(__Method_SendMessage, serviceImpl.SendMessage)
          .AddMethod(__Method_QuitRoom, serviceImpl.QuitRoom)
          .AddMethod(__Method_QuickPair, serviceImpl.QuickPair)
          .AddMethod(__Method_JoinRoom, serviceImpl.JoinRoom).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, RoomServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_CreateRoom, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(serviceImpl.CreateRoom));
      serviceBinder.AddMethod(__Method_GetRoomList, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(serviceImpl.GetRoomList));
      serviceBinder.AddMethod(__Method_GetRoomInfo, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Room>(serviceImpl.GetRoomInfo));
      serviceBinder.AddMethod(__Method_UpdateRoom, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(serviceImpl.UpdateRoom));
      serviceBinder.AddMethod(__Method_ServerBroadcast, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.RoomMsg>(serviceImpl.ServerBroadcast));
      serviceBinder.AddMethod(__Method_SendMessage, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomMsg, global::ULZAsset.ProtoMod.Empty>(serviceImpl.SendMessage));
      serviceBinder.AddMethod(__Method_QuitRoom, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Empty>(serviceImpl.QuitRoom));
      serviceBinder.AddMethod(__Method_QuickPair, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomCreateReq, global::ULZAsset.ProtoMod.Room>(serviceImpl.QuickPair));
      serviceBinder.AddMethod(__Method_JoinRoom, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ULZAsset.ProtoMod.RoomReq, global::ULZAsset.ProtoMod.Room>(serviceImpl.JoinRoom));
    }

  }
}
#endregion
