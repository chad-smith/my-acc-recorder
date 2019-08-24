using System;
using System.Net.NetworkInformation;
using Service.Requests;
using Service.Responses;

namespace Service {
  class Program {
    private static SessionPhase? phase;
    private static RaceSessionType? sessionType;

    static void Main( string[] args ) {
      Logger.CurrentLevel = LogLevel.Info;

      var accClient = new AccDataAggregator( host: "office-pc" );

      var connection = accClient.Start();

      connection.MessageReceived += ( sender, message ) => {
        if ( message is RealTimeUpdateResponse response ) {
          if ( sessionType != response.SessionType ) {
            Logger.Log( $"(Session) {sessionType} -> {response.SessionType}" );
            sessionType = response.SessionType;
          }

          if ( phase != response.Phase ) {
            Logger.Log( $"(Phase) {phase} -> {response.Phase}" );
            phase = response.Phase;
          }

/*          if ( phase != response.Phase ) {
            Logger.Log( $"(Phase) {phase} -> {response.Phase}" );
            phase = response.Phase;
          }*/
        }
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
      }

      connection.Stop();
    }
  }
}