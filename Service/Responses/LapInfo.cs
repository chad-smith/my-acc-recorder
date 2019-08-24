using System;
using System.Collections.Generic;
using System.IO;

namespace Service.Responses {
  public class LapInfo {
    public int? LaptimeMS { get; internal set; }
    public List<int?> Splits { get; } = new List<int?>();
    public ushort CarIndex { get; internal set; }
    public ushort DriverIndex { get; internal set; }
    public bool IsInvalid { get; internal set; }
    public bool IsValidForBest { get; internal set; }
    public LapType Type { get; internal set; }

    public override string ToString() {
      return $"{LaptimeMS,5}|{string.Join( "|", Splits )}";
    }

    public static LapInfo FromReader( BinaryReader reader ) {
      var lap = new LapInfo();
      lap.LaptimeMS = reader.ReadInt32();

      lap.CarIndex = reader.ReadUInt16();
      lap.DriverIndex = reader.ReadUInt16();

      var splitCount = reader.ReadByte();
      for ( int i = 0; i < splitCount; i++ )
        lap.Splits.Add( reader.ReadInt32() );

      lap.IsInvalid = reader.ReadByte() > 0;
      lap.IsValidForBest = reader.ReadByte() > 0;

      var isOutlap = reader.ReadByte() > 0;
      var isInlap = reader.ReadByte() > 0;

      if ( isOutlap )
        lap.Type = LapType.Outlap;
      else if ( isInlap )
        lap.Type = LapType.Inlap;
      else
        lap.Type = LapType.Regular;

      // Now it's possible that this is "no" lap that doesn't even include a 
      // first split, we can detect this by comparing with int32.Max
      while ( lap.Splits.Count < 3 ) {
        lap.Splits.Add( null );
      }

      // "null" entries are Int32.Max, in the C# world we can replace this to null
      for ( int i = 0; i < lap.Splits.Count; i++ )
        if ( lap.Splits[i] == Int32.MaxValue )
          lap.Splits[i] = null;

      if ( lap.LaptimeMS == Int32.MaxValue )
        lap.LaptimeMS = null;

      return lap;
    }
  }
}