// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: SSToGS.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SSToGS {

  /// <summary>Holder for reflection information generated from SSToGS.proto</summary>
  public static partial class SSToGSReflection {

    #region Descriptor
    /// <summary>File descriptor for SSToGS.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SSToGSReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxTU1RvR1MucHJvdG8SBlNTVG9HUyI4CgpBc2tQaW5nUmV0EhwKBW1nc2lk",
            "GAEgASgOMg0uU1NUb0dTLk1zZ0lEEgwKBHRpbWUYAiABKAMiPAoNQXNrUmVn",
            "aXN0ZVJldBIcCgVtZ3NpZBgBIAEoDjINLlNTVG9HUy5Nc2dJRBINCgVzdGF0",
            "ZRgCIAEoBSK3AQoNT3JkZXJQb3N0VG9HQxIcCgVtc2dpZBgBIAEoDjINLlNT",
            "VG9HUy5Nc2dJRBI2Cgt1c2VybmV0aW5mbxgCIAMoCzIhLlNTVG9HUy5PcmRl",
            "clBvc3RUb0dDLlVzZXJOZXRJbmZvEhIKCm90aGVybXNnaWQYAyABKAUSEAoI",
            "b3RoZXJtc2cYBCABKAkaKgoLVXNlck5ldEluZm8SDAoEZ3NpZBgBIAEoBRIN",
            "CgVnY25pZBgCIAEoBSI9Cg5PcmRlcktpY2tvdXRHQxIcCgVtZ3NpZBgBIAEo",
            "DjINLlNTVG9HUy5Nc2dJRBINCgVnc25pZBgCIAEoBSrXAQoFTXNnSUQSCgoG",
            "dW5rbm93EAASGgoUZU1zZ1RvR1NGcm9tU1NfQmVnaW4QgMABEh8KGWVNc2dU",
            "b0dTRnJvbVNTX0Fza1BpbmdSZXQQgcABEiIKHGVNc2dUb0dTRnJvbVNTX0Fz",
            "a1JlZ2lzdGVSZXQQgsABEiIKHGVNc2dUb0dTRnJvbVNTX09yZGVyUG9zdFRv",
            "R0MQg8ABEiMKHWVNc2dUb0dTRnJvbVNTX09yZGVyS2lja291dEdDEITAARIY",
            "ChJlTXNnVG9HU0Zyb21TU19FbmQQqMMBYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::SSToGS.MsgID), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::SSToGS.AskPingRet), global::SSToGS.AskPingRet.Parser, new[]{ "Mgsid", "Time" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::SSToGS.AskRegisteRet), global::SSToGS.AskRegisteRet.Parser, new[]{ "Mgsid", "State" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::SSToGS.OrderPostToGC), global::SSToGS.OrderPostToGC.Parser, new[]{ "Msgid", "Usernetinfo", "Othermsgid", "Othermsg" }, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::SSToGS.OrderPostToGC.Types.UserNetInfo), global::SSToGS.OrderPostToGC.Types.UserNetInfo.Parser, new[]{ "Gsid", "Gcnid" }, null, null, null)}),
            new pbr::GeneratedClrTypeInfo(typeof(global::SSToGS.OrderKickoutGC), global::SSToGS.OrderKickoutGC.Parser, new[]{ "Mgsid", "Gsnid" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum MsgID {
    [pbr::OriginalName("unknow")] Unknow = 0,
    [pbr::OriginalName("eMsgToGSFromSS_Begin")] EMsgToGsfromSsBegin = 24576,
    [pbr::OriginalName("eMsgToGSFromSS_AskPingRet")] EMsgToGsfromSsAskPingRet = 24577,
    [pbr::OriginalName("eMsgToGSFromSS_AskRegisteRet")] EMsgToGsfromSsAskRegisteRet = 24578,
    [pbr::OriginalName("eMsgToGSFromSS_OrderPostToGC")] EMsgToGsfromSsOrderPostToGc = 24579,
    [pbr::OriginalName("eMsgToGSFromSS_OrderKickoutGC")] EMsgToGsfromSsOrderKickoutGc = 24580,
    [pbr::OriginalName("eMsgToGSFromSS_End")] EMsgToGsfromSsEnd = 25000,
  }

  #endregion

  #region Messages
  public sealed partial class AskPingRet : pb::IMessage<AskPingRet> {
    private static readonly pb::MessageParser<AskPingRet> _parser = new pb::MessageParser<AskPingRet>(() => new AskPingRet());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AskPingRet> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SSToGS.SSToGSReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskPingRet() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskPingRet(AskPingRet other) : this() {
      mgsid_ = other.mgsid_;
      time_ = other.time_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskPingRet Clone() {
      return new AskPingRet(this);
    }

    /// <summary>Field number for the "mgsid" field.</summary>
    public const int MgsidFieldNumber = 1;
    private global::SSToGS.MsgID mgsid_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::SSToGS.MsgID Mgsid {
      get { return mgsid_; }
      set {
        mgsid_ = value;
      }
    }

    /// <summary>Field number for the "time" field.</summary>
    public const int TimeFieldNumber = 2;
    private long time_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Time {
      get { return time_; }
      set {
        time_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AskPingRet);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AskPingRet other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Mgsid != other.Mgsid) return false;
      if (Time != other.Time) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Mgsid != 0) hash ^= Mgsid.GetHashCode();
      if (Time != 0L) hash ^= Time.GetHashCode();
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
      if (Time != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(Time);
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
      if (Time != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Time);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AskPingRet other) {
      if (other == null) {
        return;
      }
      if (other.Mgsid != 0) {
        Mgsid = other.Mgsid;
      }
      if (other.Time != 0L) {
        Time = other.Time;
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
            mgsid_ = (global::SSToGS.MsgID) input.ReadEnum();
            break;
          }
          case 16: {
            Time = input.ReadInt64();
            break;
          }
        }
      }
    }

  }

  public sealed partial class AskRegisteRet : pb::IMessage<AskRegisteRet> {
    private static readonly pb::MessageParser<AskRegisteRet> _parser = new pb::MessageParser<AskRegisteRet>(() => new AskRegisteRet());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AskRegisteRet> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SSToGS.SSToGSReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskRegisteRet() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskRegisteRet(AskRegisteRet other) : this() {
      mgsid_ = other.mgsid_;
      state_ = other.state_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AskRegisteRet Clone() {
      return new AskRegisteRet(this);
    }

    /// <summary>Field number for the "mgsid" field.</summary>
    public const int MgsidFieldNumber = 1;
    private global::SSToGS.MsgID mgsid_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::SSToGS.MsgID Mgsid {
      get { return mgsid_; }
      set {
        mgsid_ = value;
      }
    }

    /// <summary>Field number for the "state" field.</summary>
    public const int StateFieldNumber = 2;
    private int state_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int State {
      get { return state_; }
      set {
        state_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AskRegisteRet);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AskRegisteRet other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Mgsid != other.Mgsid) return false;
      if (State != other.State) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Mgsid != 0) hash ^= Mgsid.GetHashCode();
      if (State != 0) hash ^= State.GetHashCode();
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
      if (State != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(State);
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
      if (State != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(State);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AskRegisteRet other) {
      if (other == null) {
        return;
      }
      if (other.Mgsid != 0) {
        Mgsid = other.Mgsid;
      }
      if (other.State != 0) {
        State = other.State;
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
            mgsid_ = (global::SSToGS.MsgID) input.ReadEnum();
            break;
          }
          case 16: {
            State = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class OrderPostToGC : pb::IMessage<OrderPostToGC> {
    private static readonly pb::MessageParser<OrderPostToGC> _parser = new pb::MessageParser<OrderPostToGC>(() => new OrderPostToGC());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<OrderPostToGC> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SSToGS.SSToGSReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OrderPostToGC() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OrderPostToGC(OrderPostToGC other) : this() {
      msgid_ = other.msgid_;
      usernetinfo_ = other.usernetinfo_.Clone();
      othermsgid_ = other.othermsgid_;
      othermsg_ = other.othermsg_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OrderPostToGC Clone() {
      return new OrderPostToGC(this);
    }

    /// <summary>Field number for the "msgid" field.</summary>
    public const int MsgidFieldNumber = 1;
    private global::SSToGS.MsgID msgid_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::SSToGS.MsgID Msgid {
      get { return msgid_; }
      set {
        msgid_ = value;
      }
    }

    /// <summary>Field number for the "usernetinfo" field.</summary>
    public const int UsernetinfoFieldNumber = 2;
    private static readonly pb::FieldCodec<global::SSToGS.OrderPostToGC.Types.UserNetInfo> _repeated_usernetinfo_codec
        = pb::FieldCodec.ForMessage(18, global::SSToGS.OrderPostToGC.Types.UserNetInfo.Parser);
    private readonly pbc::RepeatedField<global::SSToGS.OrderPostToGC.Types.UserNetInfo> usernetinfo_ = new pbc::RepeatedField<global::SSToGS.OrderPostToGC.Types.UserNetInfo>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::SSToGS.OrderPostToGC.Types.UserNetInfo> Usernetinfo {
      get { return usernetinfo_; }
    }

    /// <summary>Field number for the "othermsgid" field.</summary>
    public const int OthermsgidFieldNumber = 3;
    private int othermsgid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Othermsgid {
      get { return othermsgid_; }
      set {
        othermsgid_ = value;
      }
    }

    /// <summary>Field number for the "othermsg" field.</summary>
    public const int OthermsgFieldNumber = 4;
    private string othermsg_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Othermsg {
      get { return othermsg_; }
      set {
        othermsg_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as OrderPostToGC);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(OrderPostToGC other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Msgid != other.Msgid) return false;
      if(!usernetinfo_.Equals(other.usernetinfo_)) return false;
      if (Othermsgid != other.Othermsgid) return false;
      if (Othermsg != other.Othermsg) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Msgid != 0) hash ^= Msgid.GetHashCode();
      hash ^= usernetinfo_.GetHashCode();
      if (Othermsgid != 0) hash ^= Othermsgid.GetHashCode();
      if (Othermsg.Length != 0) hash ^= Othermsg.GetHashCode();
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
      usernetinfo_.WriteTo(output, _repeated_usernetinfo_codec);
      if (Othermsgid != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Othermsgid);
      }
      if (Othermsg.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Othermsg);
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
      size += usernetinfo_.CalculateSize(_repeated_usernetinfo_codec);
      if (Othermsgid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Othermsgid);
      }
      if (Othermsg.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Othermsg);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(OrderPostToGC other) {
      if (other == null) {
        return;
      }
      if (other.Msgid != 0) {
        Msgid = other.Msgid;
      }
      usernetinfo_.Add(other.usernetinfo_);
      if (other.Othermsgid != 0) {
        Othermsgid = other.Othermsgid;
      }
      if (other.Othermsg.Length != 0) {
        Othermsg = other.Othermsg;
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
            msgid_ = (global::SSToGS.MsgID) input.ReadEnum();
            break;
          }
          case 18: {
            usernetinfo_.AddEntriesFrom(input, _repeated_usernetinfo_codec);
            break;
          }
          case 24: {
            Othermsgid = input.ReadInt32();
            break;
          }
          case 34: {
            Othermsg = input.ReadString();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the OrderPostToGC message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public sealed partial class UserNetInfo : pb::IMessage<UserNetInfo> {
        private static readonly pb::MessageParser<UserNetInfo> _parser = new pb::MessageParser<UserNetInfo>(() => new UserNetInfo());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<UserNetInfo> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::SSToGS.OrderPostToGC.Descriptor.NestedTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public UserNetInfo() {
          OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public UserNetInfo(UserNetInfo other) : this() {
          gsid_ = other.gsid_;
          gcnid_ = other.gcnid_;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public UserNetInfo Clone() {
          return new UserNetInfo(this);
        }

        /// <summary>Field number for the "gsid" field.</summary>
        public const int GsidFieldNumber = 1;
        private int gsid_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int Gsid {
          get { return gsid_; }
          set {
            gsid_ = value;
          }
        }

        /// <summary>Field number for the "gcnid" field.</summary>
        public const int GcnidFieldNumber = 2;
        private int gcnid_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int Gcnid {
          get { return gcnid_; }
          set {
            gcnid_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other) {
          return Equals(other as UserNetInfo);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(UserNetInfo other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if (Gsid != other.Gsid) return false;
          if (Gcnid != other.Gcnid) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode() {
          int hash = 1;
          if (Gsid != 0) hash ^= Gsid.GetHashCode();
          if (Gcnid != 0) hash ^= Gcnid.GetHashCode();
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
          if (Gsid != 0) {
            output.WriteRawTag(8);
            output.WriteInt32(Gsid);
          }
          if (Gcnid != 0) {
            output.WriteRawTag(16);
            output.WriteInt32(Gcnid);
          }
          if (_unknownFields != null) {
            _unknownFields.WriteTo(output);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (Gsid != 0) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(Gsid);
          }
          if (Gcnid != 0) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(Gcnid);
          }
          if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(UserNetInfo other) {
          if (other == null) {
            return;
          }
          if (other.Gsid != 0) {
            Gsid = other.Gsid;
          }
          if (other.Gcnid != 0) {
            Gcnid = other.Gcnid;
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
                Gsid = input.ReadInt32();
                break;
              }
              case 16: {
                Gcnid = input.ReadInt32();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  public sealed partial class OrderKickoutGC : pb::IMessage<OrderKickoutGC> {
    private static readonly pb::MessageParser<OrderKickoutGC> _parser = new pb::MessageParser<OrderKickoutGC>(() => new OrderKickoutGC());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<OrderKickoutGC> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SSToGS.SSToGSReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OrderKickoutGC() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OrderKickoutGC(OrderKickoutGC other) : this() {
      mgsid_ = other.mgsid_;
      gsnid_ = other.gsnid_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public OrderKickoutGC Clone() {
      return new OrderKickoutGC(this);
    }

    /// <summary>Field number for the "mgsid" field.</summary>
    public const int MgsidFieldNumber = 1;
    private global::SSToGS.MsgID mgsid_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::SSToGS.MsgID Mgsid {
      get { return mgsid_; }
      set {
        mgsid_ = value;
      }
    }

    /// <summary>Field number for the "gsnid" field.</summary>
    public const int GsnidFieldNumber = 2;
    private int gsnid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Gsnid {
      get { return gsnid_; }
      set {
        gsnid_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as OrderKickoutGC);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(OrderKickoutGC other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Mgsid != other.Mgsid) return false;
      if (Gsnid != other.Gsnid) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Mgsid != 0) hash ^= Mgsid.GetHashCode();
      if (Gsnid != 0) hash ^= Gsnid.GetHashCode();
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
      if (Gsnid != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Gsnid);
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
      if (Gsnid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Gsnid);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(OrderKickoutGC other) {
      if (other == null) {
        return;
      }
      if (other.Mgsid != 0) {
        Mgsid = other.Mgsid;
      }
      if (other.Gsnid != 0) {
        Gsnid = other.Gsnid;
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
            mgsid_ = (global::SSToGS.MsgID) input.ReadEnum();
            break;
          }
          case 16: {
            Gsnid = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code