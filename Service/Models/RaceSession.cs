using System;
using System.Collections.Generic;
using Service.Responses;

namespace Service.Models {
  internal class RaceSession {
    public RaceSession( SessionType type, Guid? weekendId = null ) {
      Id = Guid.NewGuid();
      WeekendId = weekendId.GetValueOrDefault( Guid.NewGuid() );
      Type = type;
      Phase = SessionPhase.None;
      Cars = new Dictionary<int, CarDetails>();
    }

    public Guid Id { get; }
    public Guid WeekendId { get; }
    public SessionPhase Phase { get; private set; }
    public SessionType Type { get; private set; }
    public string Track { get; private set;  }
    public Dictionary<int, CarDetails> Cars { get; }
    public TimeSpan ElapsedTime { get; set; }
    public TimeSpan RemainingTime { get; set; }

    public void UpdateSessionInfo( SessionType type, SessionPhase phase ) {
      Type = type;
      Phase = phase;
    }

    public void SetTrack( string track ) {
      Track = track;
    }

    public void SetCar( int carId, CarModel carModel, (string firstName, string surname) name ) {
      var driverName = $"{name.firstName} {name.surname}";
      if ( !Cars.ContainsKey( carId ) ) {
        Cars[carId] = new CarDetails( carId, carModel, driverName );
      }
      else {
        Cars[carId].UpdateBasicDetails( carModel, driverName );
      }
    }

    public void DisconnectCar( int carId ) {
      var carDetails = Cars[carId];
      carDetails.Disconnect();
    }

    public void UpdateCarDetails( int id, ushort position, CarLocation carLocation, ushort laps, LapInfo currentLap, LapInfo lastLap, int delta ) {
      var car = Cars[id];
        
      car.Position = position;
      car.Location = carLocation;
      car.LapCount = laps;
      car.Delta = delta;
      car.CurrentLap = currentLap;
      car.LastLap = lastLap;
    }
  }
}