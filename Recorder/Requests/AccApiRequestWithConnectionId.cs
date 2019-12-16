namespace MyAcc.Recorder.Requests {
  internal abstract class AccApiRequestWithConnectionId: AccApiRequest {
    internal int ConnectionId { get; set; }
  }
}