using System.Collections.Generic;
using System.IO;

namespace MyAcc.Recorder.Responses {
  public class EntryListResponse: AccApiResponse {
    public EntryListResponse( BinaryReader reader ) {
      ConnectionId = reader.ReadInt32();
      CarCount = reader.ReadUInt16();
      CarIndices = new List<ushort>();
      for ( var i = 0; i < CarCount; i++ ) {
        CarIndices.Add( reader.ReadUInt16() );
      }
    }

    public EntryListResponse( int connectionId, ushort carCount, List<ushort> carIndices ) {
      ConnectionId = connectionId;
      CarCount = carCount;
      CarIndices = carIndices;
    }

    public int ConnectionId { get; }
    public ushort CarCount { get; }
    public List<ushort> CarIndices { get; }

    public override string ToString() {
      return $"Entry List: {CarCount} Cars: {string.Join( ", ", CarIndices )}";
    }
  }
}