using System.IO;

namespace MyAcc.Recorder.Fields {
  internal class ByteField: IApiField {
    private readonly byte _value;
    public ByteField( byte value ) {
      _value = value;
    }

    public void WriteValue( BinaryWriter writer ) {
      writer.Write( _value );
    }
  }
}