using System.Collections.Generic;
using System.IO;

namespace MyAcc.Recorder.Responses {
  public class TrackDataResponse: AccApiResponse {
    public TrackDataResponse( BinaryReader reader ) {
      ConnectionId = reader.ReadInt32();
      TrackName = reader.ReadAccString();
      TrackId = reader.ReadInt32();
      TrackMeters = reader.ReadInt32();
      CameraSets = new Dictionary<string, List<string>>();

      var cameraSetCount = reader.ReadByte();
      for ( var camSet = 0; camSet < cameraSetCount; camSet++ ) {
        var camSetName = reader.ReadAccString();
        CameraSets.Add( camSetName, new List<string>() );

        var cameraCount = reader.ReadByte();
        for ( var cam = 0; cam < cameraCount; cam++ ) {
          var cameraName = reader.ReadAccString();
          CameraSets[camSetName].Add( cameraName );
        }
      }

      var hudPages = new List<string>();
      var hudPagesCount = reader.ReadByte();
      for ( int i = 0; i < hudPagesCount; i++ ) {
        hudPages.Add( reader.ReadAccString() );
      }

      HudPages = hudPages;
    }

    public TrackDataResponse( int connectionId, List<string> hudPages, Dictionary<string, List<string>> cameraSets, int trackMeters, int trackId, string trackName ) {
      ConnectionId = connectionId;
      HudPages = hudPages;
      CameraSets = cameraSets;
      TrackMeters = trackMeters;
      TrackId = trackId;
      TrackName = trackName;
    }

    public int ConnectionId { get; }
    public List<string> HudPages { get; }
    public Dictionary<string, List<string>> CameraSets { get; }
    public int TrackMeters { get; }
    public int TrackId { get; }
    public string TrackName { get; }

    public override string ToString() {
      return $"Track Data: {TrackName} - {TrackMeters}m";
    }
  }
}