using System;
using System.Threading.Tasks;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Connection {
  public interface IAccConnection {
    Task Connect();
    void Disconnect();
    void Send( AccApiRequest request );
    bool Connected { get; }
    event EventHandler<AccApiResponse> MessageReceived;
    event EventHandler ConnectionEstablished;
    event EventHandler<ConnectionLostEventArgs> ConnectionLost;

    //TODO: Change these to abstract data entities
    Task<TrackDataResponse> GetTrackData();
    Task<EntryListResponse> GetEntryList();
  }
}