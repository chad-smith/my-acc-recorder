using System;
using System.IO;
using System.Text;

namespace MyAcc.Recorder.Fields {
  internal class StringField: IApiField {
    private readonly string _value;

    public StringField( string value ) {
      _value = value;
    }

    public void WriteValue( BinaryWriter writer ) {
      var bytes = Encoding.UTF8.GetBytes( _value );
      writer.Write( Convert.ToUInt16( bytes.Length ) );
      writer.Write( bytes );
    }
  }
}