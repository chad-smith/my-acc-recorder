namespace Service.Requests {
  internal abstract class AccApiRequestWithConnectionId: AccApiRequest {
    internal int ConnectionId { get; set; }
  }
}