using System;
using System.Linq;
using MyAcc.Recorder.Connection;
using MyAcc.Recorder.Enums;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder {
  public class AccManager {
    private static SessionManager _sessionManager;
    private readonly IAccUdpConnection _udpConnection;

    public AccManager( IAccUdpConnection udpConnection ) {
      _udpConnection = udpConnection;

      _udpConnection.MessageReceived += AccMessageReceived;
      _udpConnection.ConnectionLost += ( sender, args ) => {
        _sessionManager.AbandonSession();
        Logger.Log( "Session abandoned" );
      };

      _sessionManager = new SessionManager();

      _sessionManager.SessionStarted += ( sender, args ) => {
        _udpConnection.Send( new TrackDataRequest() );
      };
    }

    public void Start() {
      _udpConnection.Start();

      // Entry list is polled to check for car disconnections
      var entryListUpdateTimer = new System.Timers.Timer( 5000 );
      entryListUpdateTimer.Elapsed += ( args, sender ) => {
        if ( _udpConnection.Connected ) {
          _udpConnection.Send( new EntryListRequest() );
        }
      };

      entryListUpdateTimer.Start();
    }

    public void Stop() {
      _udpConnection.Stop();
    }

    private void AccMessageReceived( object sender, AccApiResponse message ) {
      if ( message is RealTimeUpdateResponse realtimeUpdate ) {
        RealTimeUpdateReceived( realtimeUpdate );
      }

      if ( message is RealTimeCarUpdateResponse realtimeCarUpdate ) {
        RealtimeCarUpdateReceived( realtimeCarUpdate );
      }

      if ( message is TrackDataResponse trackResponse ) {
        TrackResponseReceived( trackResponse );
      }

      if ( message is EntryListResponse entryListResponse ) {
        EntryListReceived( entryListResponse );
      }

      if ( message is EntryListCarResponse carResponse ) {
        EntryListCarResponseReceived( carResponse );
      }

      if ( message is BroadcastingEventResponse broadcastingEvent ) {
        BroadcastingEventReceived( broadcastingEvent );
      }
    }

    private void TrackResponseReceived( TrackDataResponse trackResponse ) {
      _sessionManager.SetTrack( trackResponse.TrackName );
      // Invert this so the SessionManager indicates it is ready for the entry list
      _udpConnection.Send( new EntryListRequest() );
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

    private static void EntryListReceived( EntryListResponse entryListResponse ) {
      _sessionManager.VerifyCarList( entryListResponse.CarIndices.Select( Convert.ToInt32 ).ToArray() );
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