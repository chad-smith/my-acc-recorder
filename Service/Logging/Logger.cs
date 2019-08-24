using System;

namespace Service {
  internal static class Logger {
    public static LogLevel CurrentLevel = LogLevel.Warning;

    public static void Log( string message, Severity severity = Severity.Info ) {
      if ( (int)CurrentLevel >= (int)severity ) {
        Console.WriteLine( $"[{severity.ToString()}] {message}");
      }
    }
  }
}