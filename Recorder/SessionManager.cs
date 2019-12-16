using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Timers;
using MyAcc.Recorder.Enums;
using MyAcc.Recorder.Models;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder {
  internal class SessionManager {
    private RaceSession _session;
    private readonly List<int> _carsPendingUpdate = new List<int>();
    public event EventHandler SessionStarted;
    public event EventHandler SessionEnded;
    public event EventHandler TimedPhaseStarted;
    public event EventHandler TimedPhaseEnded;

    public SessionManager() {
      var updateServerTimer = new Timer( 5000 ) {
        AutoReset = true
      };

      updateServerTimer.Elapsed += ( sender, args ) => {
        if ( _session != null ) {
          SendCarUpdates();
        }
      };
      updateServerTimer.Start();
    }

    public void UpdateSessionBasics( SessionType type, SessionPhase phase ) {
      if ( phase == SessionPhase.None ) {
        return;
      }

      if ( _session == null ) {
        _session = CreateSession( type );
        Logger.Log( $"{type} session started - new weekend" );
        SessionStarted?.Invoke( this, EventArgs.Empty );
      }
      else if ( type != _session.Type ) {
        if ( IsInProgress( _session.Phase ) ) {
          EndTimedPhase();
        }

        Logger.Log( $"{_session.Type} session ended" );
        SessionEnded?.Invoke( this, EventArgs.Empty );

        var isPartOfWeekend = type > _session.Type;

        _session = isPartOfWeekend
          ? CreateSession( type, _session.WeekendId )
          : CreateSession( type );

        var message = isPartOfWeekend
          ? $"{type} session started - part of existing weekend"
          : $"{type} session started - new weekend";

        Logger.Log( message, Severity.Verbose );
        SessionStarted?.Invoke( this, EventArgs.Empty );
      }

      // We capture this so we the nested methods can use the up-to-date session
      var oldPhase = _session.Phase;

      _session.UpdateSessionInfo( type, phase );

      // If we are in the same session, check if we are advancing through the timed phases
      if ( type == _session.Type ) {
        if ( IsStarting( oldPhase ) && IsInProgress( _session.Phase ) ) {
          StartTimedPhase();
        }

        if ( IsInProgress( oldPhase ) && IsEnding( _session.Phase ) ) {
          EndTimedPhase();
        }
      }
    }

    private RaceSession CreateSession( SessionType type, Guid? weekendId = null ) {
      var session = new RaceSession( type, weekendId );
      return session;
    }

    private void StartTimedPhase() {
      Logger.Log( "Timing phase started" );
      TimedPhaseStarted?.Invoke( this, EventArgs.Empty );
    }

    private void EndTimedPhase() {
      Logger.Log( "Timing phase ended" );
      TimedPhaseEnded?.Invoke( this, EventArgs.Empty );
    }

    private void SendSessionUpdates() {
      var content = new {
        Phase = _session.Phase.ToString(),
        _session.RemainingTime,
        _session.ElapsedTime,
        CarCount = _session.Cars.Count
      };

      Send( $"session/{_session.Id}/update", content );
    }

    private void SendCarUpdates() {
      //lock ( _carsPendingUpdate ) {
        if ( !_carsPendingUpdate.Any() ) {
          return;
        }

        var cars = _session.Cars.Values
          .Where( c => _carsPendingUpdate.Contains( c.Id ) )
          .ToList();

        Send(
          $"session/{_session.Id}/cars/update",
          cars.Select( car => new {
              car.Id,
              car.CurrentDriver,
              car.LapCount,
              car.Disconnected,
              CarModel = car.CarModel.GetDescription(),
              car.Delta,
              Location = car.Location.GetDescription(),
              car.Position
            }
          ).ToList()
        );

        _carsPendingUpdate.Clear();
      //}
    }

    private void Send( string endpoint, object content, HttpMethod method = null ) {
      if ( _session.Track == null ) {
        Logger.Log( "Track details not known. Skipping send", Severity.Verbose );
        return;
      }

      Logger.Log( $"Sending to {endpoint}" );

      var httpClient = new HttpClient();

      var serializedContent = JsonSerializer.Serialize( content );

      var stringContent = new StringContent(
        serializedContent,
        Encoding.Default,
        "application/json"
      );
      var message = new HttpRequestMessage(
        method ?? HttpMethod.Post,
        //$"https://my-acc.net/api/{endpoint}"
        $"http://localhost:5000/api/{endpoint}"
      );
      message.Content = stringContent;

      httpClient.SendAsync( message ).ContinueWith(
          task => {
            if ( task.IsFaulted ) {
              Logger.Log( task.Exception.Message, Severity.Error );
            }
          }
        )
        .ConfigureAwait( false );
    }

    private bool IsStarting( SessionPhase phase ) => phase < SessionPhase.Session;

    private bool IsInProgress( SessionPhase phase ) =>
      phase == SessionPhase.Session || phase == SessionPhase.SessionOver;

    private bool IsEnding( SessionPhase phase ) => phase > SessionPhase.SessionOver;

    public void SetTrack( string trackName ) {
      if ( _session.Track == null ) {
        _session.SetTrack( trackName );
        SendInitialDetails();
      }
    }

    private void SendInitialDetails() {
      var content = new {
        _session.Id,
        Type = _session.Type.GetDescription(),
        Phase = _session.Phase.GetDescription(),
        _session.RemainingTime,
        _session.ElapsedTime,
        CarCount = _session.Cars.Count,
        _session.Track
      };

      Send( "sessions", content );
    }

    public List<CarDetails> GetAllCars() => _session.Cars.Values.ToList();

    public List<CarDetails> GetActiveCars() => _session.Cars.Values.Where( v => !v.Disconnected ).ToList();

    public void SetCar( int carId, CarModel carModel, (string firstName, string surname) name ) {
      _session.SetCar( carId, carModel, name );
    }

    public void VerifyCarList( int[] carIdList ) {
      foreach ( var carKey in _session.Cars.Keys ) {
        if ( !carIdList.Contains( carKey ) ) {
          if ( !_session.Cars[carKey].Disconnected ) {
            _session.DisconnectCar( carKey );
            _carsPendingUpdate.Add( carKey );
            SendCarUpdates();
          }
        }
      }
    }

    public void AbandonSession() {
      _session = null;
    }

    public void UpdateCarDetails( ushort id, ushort position, CarLocation carLocation, ushort laps, LapInfo currentLap, LapInfo lastLap, int delta ) {
      if ( !_session.Cars.ContainsKey( id ) ) {
        return;
      }

      _session.UpdateCarDetails(
        id,
        position,
        carLocation,
        laps,
        currentLap,
        lastLap,
        delta
      );

      _carsPendingUpdate.Add( id );
    }

    public void UpdateSessionDetails( RealTimeUpdateResponse realtimeUpdate ) {
      _session.ElapsedTime = realtimeUpdate.SessionTime;
      _session.RemainingTime = realtimeUpdate.SessionEndTime;

      SendSessionUpdates();
    }
  }
}