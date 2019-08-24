using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Service.Requests;
using Service.Responses;

namespace Service {
  internal class AccDataConnection {
    private readonly string _host;
    private readonly int _port;
    private readonly UdpClient _client;
    private readonly CancellationToken _cancellationToken;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public AccDataConnection( string host, int port ) {
      _host = host;
      _port = port;
      _client = new UdpClient();
      _cancellationTokenSource = new CancellationTokenSource();
      _cancellationToken = _cancellationTokenSource.Token;
    }

    public int ConnectionId { get; private set; }
    public event EventHandler<AccApiResponse> MessageReceived;

    public void Start() {
      _client.Connect( _host, _port );

      /*Task.Run( 
        async () => {
          while ( true ) {
            var response = await _client.ReceiveAsync();
            var s = Encoding.UTF8.GetString( response.Buffer );
            Logger.Log( s );
          }
        }
      );*/

      Task.Run( async () => {
          try {
            await ListenForMessages();
          }
          catch ( Exception ex ) {
            Logger.Log( ex.Message );
          }
        }
      );
    }

    private async Task ListenForMessages() {
      while ( !_cancellationToken.IsCancellationRequested ) {
        var response = await Task.Run(
          async () => 
          await _client.ReceiveAsync().ConfigureAwait( false ),
          _cancellationToken
        );

        var accMessage = AccApiResponse.Parse( response.Buffer );

        if ( accMessage is RegistrationResponse registration ) {
          ConnectionId = registration.ConnectionId;
        }

        Logger.Log( $"Received {accMessage}", Severity.Verbose );
        
        MessageReceived?.Invoke( this, accMessage );
      }
    }

    public void Stop() {
      _cancellationTokenSource.Cancel();
    }

    public void Send( AccApiRequest request ) {
      if ( request is AccApiRequestWithConnectionId r ) {
        r.ConnectionId = ConnectionId;
      }

      Logger.Log( $"Sending {request.GetType().Name}" );
      byte[] payload;

      using ( var stream = new MemoryStream() ) {
        using ( var writer = new BinaryWriter( stream ) ) {
          var requestFields = request.GetFields();
          foreach ( var field in requestFields ) {
            field.WriteValue( writer );
          }
        }

        payload = stream.ToArray();
      }

      _client.SendAsync( payload, payload.Length );
    }
  }

}