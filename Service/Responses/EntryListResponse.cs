using System.Collections.Generic;
using System.IO;

namespace Service.Responses {
  internal class EntryListResponse: AccApiResponse {
    public EntryListResponse( BinaryReader reader ) {
      ConnectionId = reader.ReadInt32();
      CarCount = reader.ReadUInt16();
      CarIndices = new List<ushort>();
      for ( var i = 0; i < CarCount; i++ ) {
        CarIndices.Add( reader.ReadUInt16() );
      }
    }

    public int ConnectionId { get; }
    public ushort CarCount { get; }
    public List<ushort> CarIndices { get; }

    public override string ToString() {
      return $"{CarCount} {string.Join( ", ", CarIndices )}";
    }
  }
}