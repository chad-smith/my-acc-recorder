using System.Collections.Generic;
using MyAcc.Recorder.Fields;

namespace MyAcc.Recorder.Requests {
  public abstract class AccApiRequest {
    public abstract IEnumerable<IApiField> GetFields();
  }
}