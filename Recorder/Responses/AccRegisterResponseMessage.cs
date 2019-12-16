using System.IO;
using MyAcc.Recorder.Helpers;

namespace MyAcc.Recorder.Responses {
  public class RegistrationResponse: AccApiResponse {
    public RegistrationResponse( BinaryReader responseReader ) {
      ConnectionId = responseReader.ReadInt32();
      ConnectionSuccess = responseReader.ReadByte() > 0;
      IsReadonly = responseReader.ReadByte() == 0;
      ErrorMessage = responseReader.ReadAccString();
    }

    public RegistrationResponse( int connectionId, bool connectionSuccess ) {
      ConnectionId = connectionId;
      ConnectionSuccess = connectionSuccess;
    }

    public int ConnectionId { get; set; }
    public bool ConnectionSuccess { get; set; }
    public bool IsReadonly { get; set; }
    public string ErrorMessage { get; set; }

    public override string ToString() {
      var connectStatus = ConnectionSuccess ? "Succeeded" : "Failed";
      return $"Registration Response - {ConnectionId}: {connectStatus}";
    }
  }
}