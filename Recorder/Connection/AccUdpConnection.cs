using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Connection { 
  public class AccUdpConnection : IAccConnection {
    private static readonly TimeSpan DefaultUpdateInterval = TimeSpan.FromMilliseconds( 500 );
    private readonly TimeSpan _updateInterval;
    private readonly string _host;
    private readonly int _port;
    private readonly string _password;
    private readonly UdpClient _client;
    private readonly CancellationToken _cancellationToken;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Timer _noMessagesReceivedTimer;
    private bool _registered;
    private bool _sendingPing;
    private bool _retrying;

    private TaskCompletionSource<bool> _connectPromise;
    private TaskCompletionSource<TrackDataResponse> _trackDataPromise;
    private TaskCompletionSource<EntryListResponse> _entryListPromise;

    public AccUdpConnection( string host = "localhost", int port = 9000, string password = "asd", TimeSpan? updateInterval = null ) {
      _host = host;
      _port = port;
      _password = password;
      _updateInterval = updateInterval.GetValueOrDefault( DefaultUpdateInterval );
      _client = new UdpClient();
      _cancellationTokenSource = new CancellationTokenSource();
      _cancellationToken = _cancellationTokenSource.Token;

      _noMessagesReceivedTimer = new System.Timer();
      _noMessagesReceivedTimer.Elapsed += NoMessagesReceivedTimerOnElapsed();

      MessageReceived += OnMessageReceived;

    }

    private ElapsedEventHandler NoMessagesReceivedTimerOnElapsed() {
      return ( _, __ ) => {
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

    private void OnMessageReceived( object sender, AccApiResponse e ) {
      switch ( e ) {
        case RegistrationResponse registration:
          ConnectionId = registration.ConnectionId;
          ConnectionEstablished?.Invoke( this, null );
          SetConnected();
          _registered = true;
          _connectPromise.SetResult( true );
          break;
        case TrackDataResponse trackData: 
          _trackDataPromise.SetResult( trackData );
          break;
        case EntryListResponse entryList:
          _entryListPromise.SetResult( entryList );
          break;
      }

      //TODO: handle track and event list response
    }

    public bool Connected { get; private set; }
    public int ConnectionId { get; private set; }
    public event EventHandler<AccApiResponse> MessageReceived;
    public event EventHandler ConnectionEstablished;
    public event EventHandler<ConnectionLostEventArgs> ConnectionLost;

    public Task Connect() {
      _client.Connect( _host, _port );

      StartListenThread();
      
      Logger.Log( "Connecting..." );
      SendRegistrationRequest();
      
      _connectPromise = new TaskCompletionSource<bool>();
      return _connectPromise.Task;
    }

    public Task<TrackDataResponse> GetTrackData() {
      _trackDataPromise = new TaskCompletionSource<TrackDataResponse>();
      Send( new TrackDataRequest() );
      return _trackDataPromise.Task;
    }

    public Task<EntryListResponse> GetEntryList() {
      _entryListPromise = new TaskCompletionSource<EntryListResponse>();
      Send( new EntryListRequest() );
      return _entryListPromise.Task;
    }

    private void StartListenThread() {
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

        // Trim out realtime responses as this generates too much noise
        if ( !( accMessage is RealTimeUpdateResponse || accMessage is RealTimeCarUpdateResponse ) ) {
          Logger.Log( $"Received {accMessage}", Severity.Verbose );
        }
        
        MessageReceived?.Invoke( this, accMessage );
      }
    }

    public void Disconnect() {
      _cancellationTokenSource.Cancel();
    }

    public void Send( AccApiRequest request ) {
      if ( request is AccApiRequestWithConnectionId r ) {
        r.ConnectionId = ConnectionId;
      }

      if ( request is RegistrationRequest registrationRequest ) {
        _noMessagesReceivedTimer.Interval = registrationRequest.UpdateInterval
          .GetValueOrDefault( DefaultUpdateInterval )
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