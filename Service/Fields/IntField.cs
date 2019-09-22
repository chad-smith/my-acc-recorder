using System.IO;

namespace MyAcc.Recorder.Fields {
  internal class IntField: IApiField {
    private readonly int _value;

    public IntField( int value ) {
      _value = value;
    }

    public void WriteValue( BinaryWriter writer ) {
      writer.Write( _value );
    }
  }
}