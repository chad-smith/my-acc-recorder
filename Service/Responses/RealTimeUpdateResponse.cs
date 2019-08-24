using System;
using System.IO;
using System.Text;

namespace Service.Responses {
  internal class RealTimeUpdateResponse: AccApiResponse {
    public int EventIndex { get; internal set; }
    public int SessionIndex { get; internal set; }
    public SessionPhase Phase { get; internal set; }
    public TimeSpan SessionTime { get; internal set; }
    public TimeSpan TimeOfDay { get; internal set; }
    public float RainLevel { get; internal set; }
    public float Clouds { get; internal set; }
    public float Wetness { get; internal set; }
    public LapInfo BestSessionLap { get; internal set; }
    public int FocusedCarIndex { get; internal set; }
    public string ActiveCameraSet { get; internal set; }
    public string ActiveCamera { get; internal set; }
    public bool IsReplayPlaying { get; internal set; }
    public float ReplaySessionTime { get; internal set; }
    public float ReplayRemainingTime { get; internal set; }
    public TimeSpan SessionEndTime { get; internal set; }
    public RaceSessionType SessionType { get; internal set; }
    public byte AmbientTemp { get; internal set; }
    public byte TrackTemp { get; internal set; }
    public string CurrentHudPage { get; internal set; }

    public RealTimeUpdateResponse( BinaryReader reader ) {
      EventIndex = reader.ReadUInt16();
      SessionIndex = reader.ReadUInt16();
      SessionType = (RaceSessionType)reader.ReadByte();
      Phase = (SessionPhase)reader.ReadByte();
      var sessionTime = reader.ReadSingle();
      SessionTime = TimeSpan.FromMilliseconds( sessionTime );
      var sessionEndTime = reader.ReadSingle();
      SessionEndTime = TimeSpan.FromMilliseconds( sessionEndTime );

      FocusedCarIndex = reader.ReadInt32();
      ActiveCameraSet = ReadString( reader );
      ActiveCamera = ReadString( reader );
      CurrentHudPage = ReadString( reader );

      IsReplayPlaying = reader.ReadByte() > 0;
      if ( IsReplayPlaying ) {
        ReplaySessionTime = reader.ReadSingle();
        ReplayRemainingTime = reader.ReadSingle();
      }

      TimeOfDay = TimeSpan.FromMilliseconds( reader.ReadSingle() );
      AmbientTemp = reader.ReadByte();
      TrackTemp = reader.ReadByte();
      Clouds = reader.ReadByte() / 10.0f;
      RainLevel = reader.ReadByte() / 10.0f;
      Wetness = reader.ReadByte() / 10.0f;

      BestSessionLap = LapInfo.FromReader( reader );
    }

    public override string ToString() {
      return $"RealtimeUpdate: {SessionType} {Phase} {SessionEndTime}";
    }
    
    private static string ReadString( BinaryReader reader ) {
      var length = reader.ReadUInt16();
      var bytes = reader.ReadBytes( length );
      return Encoding.UTF8.GetString( bytes );
    }
  }
}