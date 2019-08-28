using System;

namespace Service {
  internal class AccDataAggregator {
    private readonly AccDataConnection _accDataConnection;
    private bool _retrying;

    public AccDataAggregator( string password = "asd", int port = 9000, TimeSpan? updateInterval = null, string host = "localhost" ) {
      _accDataConnection = new AccDataConnection( host, port, password, updateInterval );
      _accDataConnection.ConnectionLost += (_, args) => Reconnect();
      _accDataConnection.ConnectionEstablished += ( _, __ ) => Connected();
    }

    public AccDataConnection Start() {
      _accDataConnection.Start();

      return _accDataConnection;
    }

    private void Reconnect() {
      if ( !_retrying ) {
        Logger.Log( "Lost connection. Reconnecting..." );
      }
      else {
        _retrying = true;
        Logger.Log( "Connection attempt failed, retrying...", Severity.Verbose );
      }
    }

    private void Connected() {
      if ( !_retrying ) {
        Logger.Log( "Connected" );
      }
      else {
        Logger.Log( "Connected", Severity.Verbose );
      }

      _retrying = false;
    }

  }
}