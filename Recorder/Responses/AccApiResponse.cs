using System.IO;

namespace MyAcc.Recorder.Responses {
  public abstract class AccApiResponse {
    public static AccApiResponse Parse( byte[] responseBuffer ) {
      using ( var stream = new MemoryStream( responseBuffer ) ) {
        using ( var reader = new BinaryReader( stream ) ) {
          return GetResponse( reader );
        }
      }
    }

    private static AccApiResponse GetResponse( BinaryReader reader ) {
      var firstByte = reader.ReadByte();

      switch ( firstByte ) {
        case 1:
          return new RegistrationResponse( reader );
        case 2:
          return new RealTimeUpdateResponse( reader );
        case 3:
          return new RealTimeCarUpdateResponse( reader );
        case 4:
          return new EntryListResponse( reader );
        case 5:
          return new TrackDataResponse( reader );
        case 6:
          return new EntryListCarResponse( reader );
        case 7:
          return new BroadcastingEventResponse( reader );
      }

      return new UnknownResponse();
    }
  }
}