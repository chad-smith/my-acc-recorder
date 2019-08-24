using System.IO;
using Service.Requests;

namespace Service.Fields {
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