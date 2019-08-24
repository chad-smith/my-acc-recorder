using System.Collections.Generic;

namespace Service.Requests {
  internal abstract class AccApiRequest {
    public abstract IEnumerable<IApiField> GetFields();
  }
}