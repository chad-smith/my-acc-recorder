using System;
using System.Linq;
using Service.Models;
using Service.Requests;
using Service.Responses;

namespace Service {
  class Program {
    private const LogLevel DefaultLogLevel = LogLevel.Info;
    private static AccDataAggregator _accClient;
    private static SessionManager _sessionManager;
    private static AccDataConnection _connection;

    static void Main() {
      Logger.CurrentLevel = DefaultLogLevel;

      _accClient = new AccDataAggregator( host: "office-pc" );

      _connection = _accClient.Start();
      _sessionManager = new SessionManager();

      _sessionManager.TimedPhaseStarted += ( sender, args ) => {
      };

      _sessionManager.SessionStarted += ( sender, args ) => {
        _connection.Send( new TrackDataRequest() );
      };

      _connection.MessageReceived += ( sender, message ) => {
        if ( message is RealTimeUpdateResponse realtimeUpdate ) {
          _sessionManager.UpdateSessionBasics( realtimeUpdate.SessionType, realtimeUpdate.Phase );
          _sessionManager.UpdateSessionDetails( realtimeUpdate );
        }

        if ( message is RealTimeCarUpdateResponse realtimeCarUpdate ) {
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

        if ( message is TrackDataResponse trackResponse ) {
          _sessionManager.SetTrack( trackResponse.TrackName );
          // Invert this so the SessionManager indicates it is ready for the entry list
          _connection.Send( new EntryListRequest() );
        }

        if ( message is EntryListCarResponse carResponse ) {
          _sessionManager.SetCar(
            carResponse.CarId,
            carResponse.CarModel,
            (
              carResponse.Drivers.First().FirstName,
              carResponse.Drivers.First().LastName
            )
          );
        }

        if ( message is BroadcastingEventResponse broadcastingEvent ) {
          if ( broadcastingEvent.MessageType == BroadcastingEventType.LapCompleted ) {
            //TODO: Ask session manager to update car
            //_sessionManager.SendDetails();
          }
        }

        if ( message is EntryListResponse entryListResponse ) {
          _sessionManager.VerifyCarList( entryListResponse.CarIndices.Select( Convert.ToInt32 ) );
        }
      };

      _connection.ConnectionLost += ( sender, args ) => {
        _sessionManager.AbandonSession();
        Logger.Log( "Session abandoned" );
      };

      var entryListUpdateTimer = new System.Timers.Timer( 2000 );
      entryListUpdateTimer.Elapsed += ( args, sender ) => {
        if ( _connection.Connected ) {
          // This is the ping message
          _connection.Send( new EntryListRequest( true ) );
        }
      };

      entryListUpdateTimer.Start();

      while ( true ) {
        var key = Console.ReadKey().Key;
        if ( key == ConsoleKey.Q ) {
          break;
        }

        if ( key == ConsoleKey.Delete ) {
          Console.Clear();
        }

        if ( key == ConsoleKey.C ) {
          foreach ( var carDetails in _sessionManager.GetAllCars().OrderBy( c => c.Disconnected ).ThenBy( c => c.Position ) ) {
            var position = carDetails.Disconnected ? "DIS" : carDetails.Position.Value.ToString();
            Logger.Log( $"{position}. [{carDetails.Location.GetDescription()}] {carDetails.CarModel.GetDescription()} {carDetails.CurrentDriver} {carDetails.LapCount} Laps {carDetails.Delta}" );
          }
        }

        if ( key == ConsoleKey.T ) {
          _connection.Send( new TrackDataRequest() );
        }

        if ( key == ConsoleKey.E ) {
          _connection.Send( new EntryListRequest() );
        }
        
        if ( key == ConsoleKey.V ) {
          Logger.CurrentLevel = Logger.CurrentLevel == DefaultLogLevel
            ? LogLevel.Verbose
            : DefaultLogLevel;
        }
      }

      _connection.Stop();
    }
  }
}