using System;
using MyAcc.Recorder.Connection;

namespace MyAcc.Recorder {
  internal class Program {
    private const LogLevel DefaultLogLevel = LogLevel.Info;

    static void Main() {
      Logger.CurrentLevel = DefaultLogLevel;

      var udpConnection = new AccUdpConnection( "office-pc" );

      var manager = new AccManager( udpConnection );
      
      manager.Stop();
    }
  }
}