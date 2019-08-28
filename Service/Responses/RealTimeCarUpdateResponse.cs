using System;
using System.IO;

namespace Service.Responses {
  internal class RealTimeCarUpdateResponse: AccApiResponse {
    public RealTimeCarUpdateResponse( BinaryReader reader ) {
      CarIndex = reader.ReadUInt16();
      DriverIndex = reader.ReadUInt16(); // Driver swap will make this change
      DriverCount = reader.ReadByte();
      Gear = reader.ReadByte() - 2; // -2 makes the R -1, N 0 and the rest as-is
      WorldPosX = reader.ReadSingle();
      WorldPosY = reader.ReadSingle();
      Yaw = reader.ReadSingle();
      CarLocation = (CarLocation)reader.ReadByte(); // - , Track, Pitlane, PitEntry, PitExit = 4
      Kmh = reader.ReadUInt16();
      Position = reader.ReadUInt16(); // official P/Q/R position (1 based)
      CupPosition = reader.ReadUInt16(); // official P/Q/R position (1 based)
      TrackPosition = reader.ReadUInt16(); // position on track (1 based)
      SplinePosition = reader.ReadSingle(); // track position between 0.0 and 1.0
      Laps = reader.ReadUInt16();

      Delta = reader.ReadInt32(); // Realtime delta to best session lap
      BestSessionLap = LapInfo.FromReader( reader );
      LastLap = LapInfo.FromReader( reader );
      CurrentLap = LapInfo.FromReader( reader );
    }

    public override string ToString() {
      return $"RealtimeCarUpdate: {CarIndex} {CarLocation} {Position} {TimeSpan.FromMilliseconds(Convert.ToInt32(CurrentLap.LaptimeMS))}";
    }

    public LapInfo CurrentLap { get; }
    public LapInfo LastLap { get; set; }
    public LapInfo BestSessionLap { get; set; }
    public int Delta { get; set; }
    public ushort Laps { get; set; }
    public float SplinePosition { get; set; }
    public ushort TrackPosition { get; set; }
    public ushort CupPosition { get; set; }
    public ushort Position { get; set; }
    public ushort Kmh { get; set; }
    public float Yaw { get; set; }
    public float WorldPosY { get; set; }
    public float WorldPosX { get; set; }
    public int Gear { get; set; }
    public byte DriverCount { get; set; }
    public ushort DriverIndex { get; set; }
    public ushort CarIndex { get; set; }
    public CarLocation CarLocation { get; set; }
  }
}