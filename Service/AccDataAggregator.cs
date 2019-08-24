using System;
using Service.Requests;

namespace Service {
  internal class AccDataAggregator {
    private readonly string _password;
    private readonly TimeSpan? _updateInterval;
    private readonly AccDataConnection _accDataConnection;

    public AccDataAggregator( string password = "asd", int port = 9000, TimeSpan? updateInterval = null, string host = "localhost" ) {
      _password = password;
      _updateInterval = updateInterval;
      _accDataConnection = new AccDataConnection( host, port );
    }

    public AccDataConnection Start() {
      Logger.Log( "Starting connection" );
      _accDataConnection.Start();
      Logger.Log( "Connection started" );

      _accDataConnection.Send( new RegistrationRequest( _password, _updateInterval ) );

      return _accDataConnection;
    }
  }
}