using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;
using Timer = System.Timers.Timer;

namespace MyAcc.Recorder.Connection { 
  public class AccUdpConnection : IAccUdpConnection {
    private readonly TimeSpan _defaultUpdateInterval = TimeSpan.FromMilliseconds( 500 );
    private readonly string _host;
    private readonly int _port;
    private readonly string _password;
    private readonly TimeSpan? _updateInterval;
    private readonly UdpClient _client;
    private readonly CancellationToken _cancellationToken;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Timer _noMessagesReceivedTimer;
    private bool _registered;
    private bool _sendingPing;
    private bool _retrying;

    public AccUdpConnection( string host = "localhost", int port = 9000, string password = "asd", TimeSpan? updateInterval = null ) {
      _host = host;
      _port = port;
      _password = password;
      _updateInterval = updateInterval.GetValueOrDefault( _defaultUpdateInterval );
      _client = new UdpClient();
      _cancellationTokenSource = new CancellationTokenSource();
      _cancellationToken = _cancellationTokenSource.Token;

      _noMessagesReceivedTimer = new Timer();
      _noMessagesReceivedTimer.Elapsed += ( _, __ ) => {
        if ( _registered && !_sendingPing ) {
          SendPing();
          _sendingPing = true;
          return;
        }

        if ( _registered ) {
          ConnectionLost?.Invoke( null, new ConnectionLostEventArgs() );
          Reconnect();
        }

        _registered = false;
        Connected = false;

        SendRegistrationRequest();
      };
    }

    public bool Connected { get; private set; }
    public int ConnectionId { get; private set; }
    public event EventHandler<AccApiResponse> MessageReceived;
    public event EventHandler ConnectionEstablished;
    public event EventHandler<ConnectionLostEventArgs> ConnectionLost;

    public void Start() {
      _client.Connect( _host, _port );

      Task.Run( 
        async () => {
          try {
            _noMessagesReceivedTimer.Start();
            await ListenForMessages();
          }
          catch ( Exception ex ) {
            Logger.Log( ex.Message, Severity.Error );
          }
        }
      );

      Logger.Log( "Connecting..." );
      SendRegistrationRequest();
    }

    private async Task ListenForMessages() {
      while ( !_cancellationToken.IsCancellationRequested ) {
        var response = await Task.Run(
          async () => 
          await _client.ReceiveAsync().ConfigureAwait( false ),
          _cancellationToken
        );

        Connected = true;
        _sendingPing = false;
        _noMessagesReceivedTimer.Stop();
        _noMessagesReceivedTimer.Start();

        var accMessage = AccApiResponse.Parse( response.Buffer );

        if ( accMessage is RegistrationResponse registration ) {
          ConnectionId = registration.ConnectionId;
          ConnectionEstablished?.Invoke( this, null );
          SetConnected();
          _registered = true;
        }

        // Trim out realtime responses as this generates too much noise
        if ( !( accMessage is RealTimeUpdateResponse || accMessage is RealTimeCarUpdateResponse ) ) {
          Logger.Log( $"Received {accMessage}", Severity.Verbose );
        }
        
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

      if ( request is RegistrationRequest registrationRequest ) {
        _noMessagesReceivedTimer.Interval = registrationRequest.UpdateInterval
          .GetValueOrDefault(_defaultUpdateInterval)
          .TotalMilliseconds
          + 250;
      }
      else if ( !(request is TrackDataRequest td && td.IsPing ) ) {
        Logger.Log( $"Sending {request.GetType().Name}", Severity.Verbose );
      }

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

      try {
        _client.SendAsync( payload, payload.Length );
      }
      catch ( Exception ex ) {
        Logger.Log( ex.Message, Severity.Error );
      }
    }

    private void SendRegistrationRequest() {
      Send( new RegistrationRequest( _password, _updateInterval ) );
    }

    private void SendPing() {
      Send( new TrackDataRequest( true ) );
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

    private void SetConnected() {
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