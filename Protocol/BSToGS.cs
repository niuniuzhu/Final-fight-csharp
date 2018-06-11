// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BSToGS.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace BSToGS {

  /// <summary>Holder for reflection information generated from BSToGS.proto</summary>
  public static partial class BSToGSReflection {

    #region Descriptor
    /// <summary>File descriptor for BSToGS.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BSToGSReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxCU1RvR1MucHJvdG8SBkJTVG9HUyJHCgtBc2tSZWdpc3RlchIcCgVtZ3Np",
            "ZBgBIAEoDjINLkJTVG9HUy5Nc2dJRBIMCgRnc2lkGAIgASgFEgwKBHBvcnQY",
            "AyABKAUigQEKEU9uZVVzZXJMb2dpblRva2VuEhwKBW1zZ2lkGAEgASgOMg0u",
            "QlNUb0dTLk1zZ0lEEhIKCmdhdGVjbGllbnQYAiABKAUSDQoFdG9rZW4YAyAB",
            "KAkSEQoJdXNlcl9uYW1lGAQgASgJEgwKBHBvcnQYBSABKAUSCgoCaXAYBiAB",
            "KAkqlgEKBU1zZ0lEEgoKBnVua25vdxAAEhoKFGVNc2dUb0dTRnJvbUJTX0Jl",
            "Z2luEIDAARIjCh1lTXNnVG9HU0Zyb21CU19Bc2tSZWdpc3RlclJldBCBwAES",
            "JgogZU1zZ1RvR1NGcm9tQlNfT25lVXNlckxvZ2luVG9rZW4QgsABEhgKEmVN",
            "c2dUb0dTRnJvbUJTX0VuZBCowwFiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::BSToGS.MsgID), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::BSToGS.AskRegister), global::BSToGS.AskRegister.Parser, new[]{ "Mgsid", "Gsid", "Port" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::BSToGS.OneUserLoginToken), global::BSToGS.OneUserLoginToken.Parser, new[]{ "Msgid", "Gateclient", "Token", "UserName", "Port", "Ip" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum MsgID {
    [pbr::OriginalName("unknow")] Unknow = 0,
    [pbr::OriginalName("eMsgToGSFromBS_Begin")] EMsgToGsfromBsBegin = 24576,
    [pbr::OriginalName("eMsgToGSFromBS_AskRegisterRet")] EMsgToGsfromBsAskRegisterRet = 24577,
    [pbr::OriginalName("eMsgToGSFromBS_OneUserLoginToken")] EMsgToGsfromBsOneUserLoginToken = 24578,
    [pbr::OriginalName("eMsgToGSFromBS_End")] EMsgToGsfromBsEnd = 25000,
  }

  #endregion

  #region Messages
  public sealed partial class AskRegister : pb::IMessage<AskRegister> {
    private static readonly pb::MessageParser<AskRegister> _parser = new pb::MessageParser<AskRegister>(() => new AskRegister());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AskRegister> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::BSToGS.BSToGSReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskRegister() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskRegister(AskRegister other) : this() {
      mgsid_ = other.mgsid_;
      gsid_ = other.gsid_;
      port_ = other.port_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskRegister Clone() {
      return new AskRegister(this);
    }

    /// <summary>Field number for the "mgsid" field.</summary>
    public const int MgsidFieldNumber = 1;
    private global::BSToGS.MsgID mgsid_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::BSToGS.MsgID Mgsid {
      get { return mgsid_; }
      set {
        mgsid_ = value;
      }
    }

    /// <summary>Field number for the "gsid" field.</summary>
    public const int GsidFieldNumber = 2;
    private int gsid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Gsid {
      get { return gsid_; }
      set {
        gsid_ = value;
      }
    }

    /// <summary>Field number for the "port" field.</summary>
    public const int PortFieldNumber = 3;
    private int port_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Port {
      get { return port_; }
      set {
        port_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AskRegister);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AskRegister other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Mgsid != other.Mgsid) return false;
      if (Gsid != other.Gsid) return false;
      if (Port != other.Port) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Mgsid != 0) hash ^= Mgsid.GetHashCode();
      if (Gsid != 0) hash ^= Gsid.GetHashCode();
      if (Port != 0) hash ^= Port.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Mgsid != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Mgsid);
      }
      if (Gsid != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Gsid);
      }
      if (Port != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Port);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Mgsid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Mgsid);
      }
      if (Gsid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Gsid);
      }
      if (Port != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Port);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AskRegister other) {
      if (other == null) {
        return;
      }
      if (other.Mgsid != 0) {
        Mgsid = other.Mgsid;
      }
      if (other.Gsid != 0) {
        Gsid = other.Gsid;
      }
      if (other.Port != 0) {
        Port = other.Port;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            mgsid_ = (global::BSToGS.MsgID) input.ReadEnum();
            break;
          }
          case 16: {
            Gsid = input.ReadInt32();
            break;
          }
          case 24: {
            Port = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class OneUserLoginToken : pb::IMessage<OneUserLoginToken> {
    private static readonly pb::MessageParser<OneUserLoginToken> _parser = new pb::MessageParser<OneUserLoginToken>(() => new OneUserLoginToken());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<OneUserLoginToken> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::BSToGS.BSToGSReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OneUserLoginToken() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OneUserLoginToken(OneUserLoginToken other) : this() {
      msgid_ = other.msgid_;
      gateclient_ = other.gateclient_;
      token_ = other.token_;
      userName_ = other.userName_;
      port_ = other.port_;
      ip_ = other.ip_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OneUserLoginToken Clone() {
      return new OneUserLoginToken(this);
    }

    /// <summary>Field number for the "msgid" field.</summary>
    public const int MsgidFieldNumber = 1;
    private global::BSToGS.MsgID msgid_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::BSToGS.MsgID Msgid {
      get { return msgid_; }
      set {
        msgid_ = value;
      }
    }

    /// <summary>Field number for the "gateclient" field.</summary>
    public const int GateclientFieldNumber = 2;
    private int gateclient_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Gateclient {
      get { return gateclient_; }
      set {
        gateclient_ = value;
      }
    }

    /// <summary>Field number for the "token" field.</summary>
    public const int TokenFieldNumber = 3;
    private string token_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Token {
      get { return token_; }
      set {
        token_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "user_name" field.</summary>
    public const int UserNameFieldNumber = 4;
    private string userName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string UserName {
      get { return userName_; }
      set {
        userName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "port" field.</summary>
    public const int PortFieldNumber = 5;
    private int port_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Port {
      get { return port_; }
      set {
        port_ = value;
      }
    }

    /// <summary>Field number for the "ip" field.</summary>
    public const int IpFieldNumber = 6;
    private string ip_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Ip {
      get { return ip_; }
      set {
        ip_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as OneUserLoginToken);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(OneUserLoginToken other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Msgid != other.Msgid) return false;
      if (Gateclient != other.Gateclient) return false;
      if (Token != other.Token) return false;
      if (UserName != other.UserName) return false;
      if (Port != other.Port) return false;
      if (Ip != other.Ip) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Msgid != 0) hash ^= Msgid.GetHashCode();
      if (Gateclient != 0) hash ^= Gateclient.GetHashCode();
      if (Token.Length != 0) hash ^= Token.GetHashCode();
      if (UserName.Length != 0) hash ^= UserName.GetHashCode();
      if (Port != 0) hash ^= Port.GetHashCode();
      if (Ip.Length != 0) hash ^= Ip.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Msgid != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Msgid);
      }
      if (Gateclient != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Gateclient);
      }
      if (Token.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Token);
      }
      if (UserName.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(UserName);
      }
      if (Port != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(Port);
      }
      if (Ip.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(Ip);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Msgid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Msgid);
      }
      if (Gateclient != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Gateclient);
      }
      if (Token.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Token);
      }
      if (UserName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(UserName);
      }
      if (Port != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Port);
      }
      if (Ip.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Ip);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(OneUserLoginToken other) {
      if (other == null) {
        return;
      }
      if (other.Msgid != 0) {
        Msgid = other.Msgid;
      }
      if (other.Gateclient != 0) {
        Gateclient = other.Gateclient;
      }
      if (other.Token.Length != 0) {
        Token = other.Token;
      }
      if (other.UserName.Length != 0) {
        UserName = other.UserName;
      }
      if (other.Port != 0) {
        Port = other.Port;
      }
      if (other.Ip.Length != 0) {
        Ip = other.Ip;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            msgid_ = (global::BSToGS.MsgID) input.ReadEnum();
            break;
          }
          case 16: {
            Gateclient = input.ReadInt32();
            break;
          }
          case 26: {
            Token = input.ReadString();
            break;
          }
          case 34: {
            UserName = input.ReadString();
            break;
          }
          case 40: {
            Port = input.ReadInt32();
            break;
          }
          case 50: {
            Ip = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code