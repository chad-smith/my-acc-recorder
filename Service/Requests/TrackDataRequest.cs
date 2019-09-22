using System.Collections.Generic;
using MyAcc.Recorder.Fields;

namespace MyAcc.Recorder.Requests {
  internal class TrackDataRequest: AccApiRequestWithConnectionId {
    public bool IsPing { get; }

    public TrackDataRequest( bool isPing = false ) {
      IsPing = isPing;
    }

    public override IEnumerable<IApiField> GetFields() {
      return new IApiField[] {
        new ByteField( 11 ),
        new IntField( ConnectionId )
      };
    }
  }
}