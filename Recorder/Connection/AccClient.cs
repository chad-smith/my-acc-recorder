using System;
using System.Threading.Tasks;
using MyAcc.Recorder.Requests;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Connection {

  //TODO: Add another layer of abstraction so that the UDP
  //connection is mocked


  /// <summary>
  /// The AccClient is the primary means by which data is requested from ACC
  /// </summary>
  public class AccClient {
    private readonly IAccConnection _connection;

    public AccClient( IAccConnection connection ) {
      _connection = connection;
    }

    public async Task Connect() {
      await _connection.Connect();
    }

    public void Disconnect() {
      _connection.Disconnect();
    }

    public Task<TrackDataResponse> GetTrackData() {
      return _connection.GetTrackData();
    }

    public Task<EntryListResponse> GetEntryList() {
      return _connection.GetEntryList();
    }

    public void Send( AccApiRequest request ) {
      _connection.Send( request );
    }
  }
}