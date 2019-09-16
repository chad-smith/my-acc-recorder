using System.Collections.Generic;
using Service.Fields;

namespace Service.Requests {
  internal abstract class AccApiRequest {
    public abstract IEnumerable<IApiField> GetFields();
  }
}