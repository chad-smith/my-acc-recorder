using System;
using System.Collections.Generic;
using MyAcc.Recorder.Connection;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Tester {
  class FakeAccUdpConnection: IAccUdpConnection {
    public void Start() {
      Connected = true;
      ConnectionId = 10;
      ConnectionEstablished?.Invoke( this, EventArgs.Empty );
    }

    public void SendTrackData() {
      var trackDataResponse = new TrackDataResponse(
        ConnectionId,
        new List<string>(),
        new Dictionary<string, List<string>>(),
        3402,
        22,
        "Brands Hatch"
      );

      MessageReceived?.Invoke( this, trackDataResponse);
    }

    public void Stop() {
      Connected = false;
      ConnectionLost?.Invoke( this, new ConnectionLostEventArgs() );
    }

    public void Send( AccApiRequest request ) {
    }

    public bool Connected { get; private set; }
    public int ConnectionId { get; set; }
    public event EventHandler<AccApiResponse> MessageReceived;
    public event EventHandler ConnectionEstablished;
    public event EventHandler<ConnectionLostEventArgs> ConnectionLost;
  }
}