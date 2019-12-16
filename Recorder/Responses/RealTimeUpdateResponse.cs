using System;
using System.IO;
using System.Text;
using MyAcc.Recorder.Enums;

namespace MyAcc.Recorder.Responses {
  public class RealTimeUpdateResponse: AccApiResponse {
    public RealTimeUpdateResponse( BinaryReader reader ) {
      EventIndex = reader.ReadUInt16();
      SessionIndex = reader.ReadUInt16();
      SessionType = (SessionType)reader.ReadByte();
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

    public RealTimeUpdateResponse( SessionPhase phase, TimeSpan sessionTime, bool isReplayPlaying, TimeSpan sessionEndTime, SessionType sessionType, byte ambientTemp, byte trackTemp ) {
      Phase = phase;
      SessionTime = sessionTime;
      IsReplayPlaying = isReplayPlaying;
      SessionEndTime = sessionEndTime;
      SessionType = sessionType;
      AmbientTemp = ambientTemp;
      TrackTemp = trackTemp;
    }

    public int EventIndex { get; }
    public int SessionIndex { get; }
    public SessionPhase Phase { get; }
    public TimeSpan SessionTime { get; }
    public TimeSpan TimeOfDay { get; }
    public float RainLevel { get; }
    public float Clouds { get; }
    public float Wetness { get; }
    public LapInfo BestSessionLap { get; }
    public int FocusedCarIndex { get; }
    public string ActiveCameraSet { get; }
    public string ActiveCamera { get; }
    public bool IsReplayPlaying { get; }
    public float ReplaySessionTime { get; }
    public float ReplayRemainingTime { get; }
    public TimeSpan SessionEndTime { get; }
    public SessionType SessionType { get; }
    public byte AmbientTemp { get; }
    public byte TrackTemp { get; }
    public string CurrentHudPage { get; }
    
    public override string ToString() {
      return $"RealtimeUpdate: {SessionType} {Phase} {SessionEndTime} {TimeOfDay}";
    }
    
    private static string ReadString( BinaryReader reader ) {
      var length = reader.ReadUInt16();
      var bytes = reader.ReadBytes( length );
      return Encoding.UTF8.GetString( bytes );
    }
  }
}