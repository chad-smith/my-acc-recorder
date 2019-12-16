using System;
using System.Linq;
using System.Threading.Tasks;
using MyAcc.Recorder.Connection;
using MyAcc.Recorder.Enums;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder {
  public class AccManager {
    private static SessionManager _sessionManager;
    private readonly AccClient _accClient;

    public AccManager( IAccConnection connection ) {
      _accClient = new AccClient( connection );

      //_connection.MessageReceived += AccMessageReceived;
      /*_connection.ConnectionLost += ( sender, args ) => {
        _sessionManager.AbandonSession();
        Logger.Log( "Session abandoned" );
      };
*/

      _sessionManager = new SessionManager();

      _sessionManager.SessionStarted += async ( sender, args ) => {
        var trackData = await _accClient.GetTrackData();
        _sessionManager.SetTrack( trackData.TrackName );
      };
    }

    public async Task Start() {
      await _accClient.Connect();

      // Entry list is polled to check for car disconnections
      var entryListUpdateTimer = new System.Timers.Timer( 5000 );
      entryListUpdateTimer.Elapsed += async ( args, sender ) => {
        var entryList = await _accClient.GetEntryList();

        _sessionManager.VerifyCarList( entryList.CarIndices.Select( Convert.ToInt32 ).ToArray() );
      };

      entryListUpdateTimer.Start();
    }

    public void Stop() {
      _accClient.Disconnect();
    }

    private void AccMessageReceived( object sender, AccApiResponse message ) {
      if ( message is RealTimeUpdateResponse realtimeUpdate ) {
        RealTimeUpdateReceived( realtimeUpdate );
      }

      if ( message is RealTimeCarUpdateResponse realtimeCarUpdate ) {
        RealtimeCarUpdateReceived( realtimeCarUpdate );
      }
      
      //TODO: bundle with entry list?
      if ( message is EntryListCarResponse carResponse ) {
        EntryListCarResponseReceived( carResponse );
      }

      if ( message is BroadcastingEventResponse broadcastingEvent ) {
        BroadcastingEventReceived( broadcastingEvent );
      }
    }
    
    private static void RealTimeUpdateReceived( RealTimeUpdateResponse realtimeUpdate ) {
      _sessionManager.UpdateSessionBasics( realtimeUpdate.SessionType, realtimeUpdate.Phase );
      _sessionManager.UpdateSessionDetails( realtimeUpdate );
    }

    private static void RealtimeCarUpdateReceived( RealTimeCarUpdateResponse realtimeCarUpdate ) {
      _sessionManager.UpdateCarDetails(
        realtimeCarUpdate.CarIndex,
        realtimeCarUpdate.Position,
        realtimeCarUpdate.CarLocation,
        realtimeCarUpdate.Laps,
        realtimeCarUpdate.CurrentLap,
        realtimeCarUpdate.LastLap,
        realtimeCarUpdate.Delta
      );
    }

    private static void EntryListCarResponseReceived( EntryListCarResponse carResponse ) {
      _sessionManager.SetCar(
        carResponse.CarId,
        carResponse.CarModel,
        (
          carResponse.Drivers.First().FirstName,
          carResponse.Drivers.First().LastName
        )
      );
    }

    private static void BroadcastingEventReceived( BroadcastingEventResponse broadcastingEvent ) {
      if ( broadcastingEvent.MessageType == BroadcastingEventType.LapCompleted ) {
        //TODO: Ask session manager to update car
        //_sessionManager.SendDetails();
      }
    }
  }
}