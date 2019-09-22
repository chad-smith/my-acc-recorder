using System;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Connection {
  public interface IAccUdpConnection {
    void Start();
    void Stop();
    void Send( AccApiRequest request );
    bool Connected { get; }
    int ConnectionId { get; }
    event EventHandler<AccApiResponse> MessageReceived;
    event EventHandler ConnectionEstablished;
    event EventHandler<ConnectionLostEventArgs> ConnectionLost;
  }

}