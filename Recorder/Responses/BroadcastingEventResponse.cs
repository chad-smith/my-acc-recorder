using System.IO;
using MyAcc.Recorder.Enums;
using MyAcc.Recorder.Helpers;

namespace MyAcc.Recorder.Responses {
  internal class BroadcastingEventResponse: AccApiResponse {
    public BroadcastingEventResponse( BinaryReader reader ) {
      MessageType = (BroadcastingEventType)reader.ReadByte();
      Message = reader.ReadAccString();
      Time = reader.ReadInt32();
      CarId = reader.ReadInt32();
    }

    public override string ToString() {
      return $"Broadcast ({MessageType.ToString()}): [{Time} / {CarId}] {Message}";
    }

    public BroadcastingEventType MessageType { get; }
    public string Message { get; }
    public int Time { get; }
    public int CarId { get; }
  }
}