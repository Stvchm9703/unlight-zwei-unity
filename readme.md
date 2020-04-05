

### RoomService
```powershell
protoc ^
    -I=.\Assets\Scripts\Protos\ ^
    -I=%proto_path%\include ^
    -I=%proto_path%\googleapis ^
    --csharp_out=.\Assets\Scripts\Protos\RoomService ^
    .\Assets\Scripts\Protos\RoomService\*.proto ^
    --grpc_out=.\Assets\Scripts\Protos\RoomService ^
    --plugin=protoc-gen-grpc=%proto_path%\bin\grpc_csharp_plugin.exe 
```


protoc -I=.\Assets\Scripts\Protos\ -I=%proto_path%\include -I=%proto_path%\googleapis --csharp_out=.\Assets\Scripts\Protos\RoomService .\Assets\Scripts\Protos\RoomService\*.proto --grpc_out=.\Assets\Scripts\Protos\RoomService --plugin=protoc-gen-grpc=%proto_path%\bin\grpc_csharp_plugin.exe 