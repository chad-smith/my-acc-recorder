using System;
using MyAcc.Recorder.Connection;

namespace MyAcc.Recorder {
  internal class Program {
    private const LogLevel DefaultLogLevel = LogLevel.Info;

    static void Main() {
      Logger.CurrentLevel = DefaultLogLevel;

      var udpConnection = new AccUdpConnection( "office-pc" );

      var manager = new AccManager( udpConnection );

      while ( true ) {
        var key = Console.ReadKey().Key;
        if ( key == ConsoleKey.Q ) {
          break;
        }

        switch ( key ) {
          case ConsoleKey.Delete:
            Console.Clear();
            break;
          case ConsoleKey.V:
            Logger.CurrentLevel = Logger.CurrentLevel == DefaultLogLevel
              ? LogLevel.Verbose
              : DefaultLogLevel;
            break;
        }
      }

      manager.Stop();
    }
  }
}