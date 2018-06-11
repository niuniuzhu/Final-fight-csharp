protoc SSToCS.proto --csharp_out=../../Protocol
protoc SSToGS.proto --csharp_out=../../Protocol

protoc GSToBS.proto --csharp_out=../../Protocol
protoc GSToSS.proto --csharp_out=../../Protocol
protoc GSToCS.proto --csharp_out=../../Protocol
protoc GSToGC.proto --csharp_out=../../Protocol

protoc BSToLS.proto --csharp_out=../../Protocol
protoc BSToGS.proto --csharp_out=../../Protocol
protoc BSToGC.proto --csharp_out=../../Protocol

protoc GCToLS.proto --csharp_out=../../Protocol
protoc GCToBS.proto --csharp_out=../../Protocol
protoc GCToCS.proto --csharp_out=../../Protocol
protoc GCToSS.proto --csharp_out=../../Protocol

protoc CSToSS.proto --csharp_out=../../Protocol
protoc CSToGS.proto --csharp_out=../../Protocol
protoc CSToRC.proto --csharp_out=../../Protocol

protoc LSToBS.proto --csharp_out=../../Protocol
protoc LSToGC.proto --csharp_out=../../Protocol

protoc RCToCS.proto --csharp_out=../../Protocol

protoc CSToDB.proto --csharp_out=../../Protocol
protoc DBToCs.proto --csharp_out=../../Protocol

protoc LSToSDK.proto --csharp_out=../../Protocol
protoc SDKToLS.proto --csharp_out=../../Protocol

protoc ToLog.proto --csharp_out=../../Protocol

rem protogen -i:GCToLS.proto -o:../ClientProtobuf/GCToLS.cs
rem protogen -i:GCToSS.proto -o:../ClientProtobuf/GCToSS.cs
rem protogen -i:GCToCS.proto -o:../ClientProtobuf/GCToCS.cs
rem protogen -i:GCToBS.proto -o:../ClientProtobuf/GCToBS.cs
rem protogen -i:GSToGC.proto -o:../ClientProtobuf/GSToGC.cs
rem protogen -i:BSToGC.proto -o:../ClientProtobuf/BSToGC.cs
rem protogen -i:LSToGC.proto -o:../ClientProtobuf/LSToGC.cs

rem protogen -i:CSToRC.proto -o:../RemoteProtobuf/CSToRC.cs
rem protogen -i:RCToCS.proto -o:../RemoteProtobuf/RCToCS.cs

rem cd ../ClientProtobuf 
rem copy * /Y ..\..\..\Client\Assets\ProtobuMsg
rem copy ..\RemoteProtobuf\CSToRC.cs /Y ..\..\RemoteConsole\RemoteConsole
rem copy ..\RemoteProtobuf\RCToCS.cs /Y ..\..\RemoteConsole\RemoteConsole
rem pause