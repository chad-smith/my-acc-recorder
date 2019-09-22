using System.Collections.Generic;
using MyAcc.Recorder.Fields;

namespace MyAcc.Recorder.Requests {
  internal class EntryListRequest: AccApiRequestWithConnectionId {
    public override IEnumerable<IApiField> GetFields() {
      return new IApiField[] {
        new ByteField( 10 ),
        new IntField( ConnectionId )
      };
    }
  }
}