using System.Collections.Generic;
using Service.Fields;

namespace Service.Requests {
  internal class TrackDataRequest: AccApiRequestWithConnectionId {

    public override IEnumerable<IApiField> GetFields() {
      return new IApiField[] {
        new ByteField( 11 ),
        new IntField( ConnectionId )
      };
    }
  }
}