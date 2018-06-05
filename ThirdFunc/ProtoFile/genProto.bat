protoc SSToCS.proto --csharp_out=../../Shared/Protocols
protoc SSToGS.proto --csharp_out=../../Shared/Protocols

protoc GSToBS.proto --csharp_out=../../Shared/Protocols
protoc GSToSS.proto --csharp_out=../../Shared/Protocols
protoc GSToCS.proto --csharp_out=../../Shared/Protocols
protoc GSToGC.proto --csharp_out=../../Shared/Protocols

protoc BSToLS.proto --csharp_out=../../Shared/Protocols
protoc BSToGS.proto --csharp_out=../../Shared/Protocols
protoc BSToGC.proto --csharp_out=../../Shared/Protocols

protoc GCToLS.proto --csharp_out=../../Shared/Protocols
protoc GCToBS.proto --csharp_out=../../Shared/Protocols
protoc GCToCS.proto --csharp_out=../../Shared/Protocols
protoc GCToSS.proto --csharp_out=../../Shared/Protocols

protoc CSToSS.proto --csharp_out=../../Shared/Protocols
protoc CSToGS.proto --csharp_out=../../Shared/Protocols
protoc CSToRC.proto --csharp_out=../../Shared/Protocols

protoc LSToBS.proto --csharp_out=../../Shared/Protocols
protoc LSToGC.proto --csharp_out=../../Shared/Protocols

protoc RCToCS.proto --csharp_out=../../Shared/Protocols

protoc CSToDB.proto --csharp_out=../../Shared/Protocols
protoc DBToCs.proto --csharp_out=../../Shared/Protocols

protoc LSToSDK.proto --csharp_out=../../Shared/Protocols
protoc SDKToLS.proto --csharp_out=../../Shared/Protocols

protoc ToLog.proto --csharp_out=../../Shared/Protocols

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