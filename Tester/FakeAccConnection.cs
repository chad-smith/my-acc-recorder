using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyAcc.Recorder.Connection;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Tester {
  public class FakeAccConnection: IAccConnection {

    public bool Connected { get; private set; }
    public int ConnectionId { get; set; }
    public event EventHandler<AccApiResponse> MessageReceived;
    public event EventHandler ConnectionEstablished;
    public event EventHandler<ConnectionLostEventArgs> ConnectionLost;

    public Task Connect() {
      Connected = true;
      ConnectionId = 10;
      ConnectionEstablished?.Invoke( this, EventArgs.Empty );
      return Task.FromResult( true );
    }

    public void Disconnect() {
      Connected = false;
      ConnectionLost?.Invoke( this, new ConnectionLostEventArgs() );
    }

    public void Send( AccApiRequest request ) {
    }

    public Task<TrackDataResponse> GetTrackData() {
      return new Task<TrackDataResponse>( () =>
        new TrackDataResponse( "Brands Hatch" )
      );
    }

    public Task<EntryListResponse> GetEntryList() {
      return new Task<EntryListResponse>( () =>
        new EntryListResponse( 10, 2, new List<ushort> {0, 1}
        )
      );
    }

    public void InvokeFakeResponse( AccApiResponse response ) {
      MessageReceived?.Invoke( this, response );
    }
  }
}