using System;
using System.Collections.Generic;
using Service.Fields;

namespace Service.Requests {
  internal class EntryListRequest: AccApiRequestWithConnectionId {
    public override IEnumerable<IApiField> GetFields() {
      return new IApiField[] {
        new ByteField( 10 ),
        new IntField( ConnectionId )
      };
    }
  }
}