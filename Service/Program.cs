using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Service.Requests;
using Service.Responses;

namespace Service {
  class Program {
    private static SessionPhase? _currentPhase;
    private static RaceSessionType? _currentSessionType;
    private const LogLevel DefaultLogLevel = LogLevel.Info;
    private static bool _sessionInProgress;
    private static Guid _sessionId;
    private static Guid _weekendId;
    private static string _track;
    private static Dictionary<int, CarDetails> _cars = new Dictionary<int, CarDetails>();
    private static int _playerCarIndex;

    static void Main() {
      Logger.CurrentLevel = DefaultLogLevel;

      var accClient = new AccDataAggregator( host: "office-pc" );

      var connection = accClient.Start();

      connection.MessageReceived += ( sender, message ) => {
        if ( message is RealTimeUpdateResponse response ) {
          if ( response.IsReplayPlaying ) {
            return;
          }

          // If there is a session in progress and we go to a pre-session phase,
          // the previous session must have been abandoned
          if ( _sessionInProgress && response.Phase < SessionPhase.Session ) {
            Logger.Log( $"{_currentSessionType} abandoned" );
            _sessionInProgress = false;
          }

          // We don't really know what to do with this, so we do nothing 
          if ( response.Phase == SessionPhase.None ) {
            return;
          }

          // Between Session and PostSession the race is in progress
          if ( response.Phase >= SessionPhase.Session && response.Phase < SessionPhase.PostSession ) {
            if ( !_sessionInProgress ) {
              Logger.Log( $"{response.SessionType} started" );
            }

            _sessionInProgress = true;
            _sessionId = Guid.NewGuid();
          }
          else {
            if ( _sessionInProgress ) {
              Logger.Log( $"{response.SessionType} finished" );

              //DEBUG!
              Logger.Log( $"Sending to server: \r\nWeekend: {_weekendId}\r\nSession: {_sessionId}" );
              var httpClient = new HttpClient();
              var content = JsonConvert.SerializeObject( new {
                  Player = _cars[_playerCarIndex].DriverName,
                  Status = "Finished",
                  StartTime = DateTime.Now,
                  DriverCount = _cars.Count,
                  Track = _track
                }
              );
              httpClient.PostAsync( "https://my-acc.net/api/sessions", new StringContent( content ) );
            }

            _sessionInProgress = false;
          }

          if ( _currentSessionType != response.SessionType ) {
            Logger.Log( $"(Session) {_currentSessionType} -> {response.SessionType}", Severity.Verbose );

            if ( response.SessionType < _currentSessionType ) {
              _weekendId = Guid.NewGuid();
              Logger.Log( "Starting new weekend" );
            }

            _currentSessionType = response.SessionType;
          }

          if ( _currentPhase != response.Phase ) {
            Logger.Log( $"(Phase) {_currentPhase} -> {response.Phase}", Severity.Verbose );
            _currentPhase = response.Phase;
          }
        }

        if ( message is RealTimeUpdateResponse r ) {
          _playerCarIndex = r.FocusedCarIndex;
        }

        if ( message is TrackDataResponse track ) {
          _track = track.TrackName;
        }

        if ( message is EntryListCarResponse carResponse ) {
          _cars[carResponse.CarId] = new CarDetails(
            carResponse.CarModel,
            carResponse.Drivers.First().LastName
          );
        }

        if ( message is TrackDataResponse || message is EntryListCarResponse || message is EntryListResponse ) {
          Logger.Log( message.ToString() );
        }
      };

      connection.ConnectionLost += ( sender, args ) => {
        if ( _sessionInProgress ) {
          Logger.Log( $"{_currentSessionType} abandoned" );
          //DEBUG!
          Logger.Log( $"Sending to server: \r\nWeekend: {_weekendId}\r\nSession: {_sessionId}" );
        }
      };

      connection.ConnectionEstablished += ( sender, args ) => {
        _weekendId = Guid.NewGuid();
        _sessionId = Guid.NewGuid();
      };

      while ( true ) {
        var key = Console.ReadKey().Key;
        if ( key == ConsoleKey.Q ) {
          break;
        }

        if ( key == ConsoleKey.Delete || key == ConsoleKey.C ) {
          Console.Clear();
        }

        if ( key == ConsoleKey.T ) {
          connection.Send( new TrackDataRequest() );
        }

        if ( key == ConsoleKey.E ) {
          connection.Send( new EntryListRequest() );
        }

        if ( key == ConsoleKey.D ) {
          Logger.Log( $"{_track} {_cars[_playerCarIndex].DriverName}: {string.Join( ", ", _cars.Select( c => $"{c.Key} {c.Value.DriverName}"))}" );
        }

        if ( key == ConsoleKey.V ) {
          Logger.CurrentLevel = Logger.CurrentLevel == DefaultLogLevel
            ? LogLevel.Verbose
            : DefaultLogLevel;
        }
      }

      connection.Stop();
    }
  }

  internal class CarDetails {
    public CarModel CarModel { get; }
    public string DriverName { get; }

    public CarDetails( CarModel carModel, string driverName ) {
      CarModel = carModel;
      DriverName = driverName;
    }
  }
}