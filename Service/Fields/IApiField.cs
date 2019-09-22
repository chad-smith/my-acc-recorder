using System.IO;

namespace MyAcc.Recorder.Fields {
  public interface IApiField {
    void WriteValue( BinaryWriter writer );
  }
}