using System.IO;
using System.Text;

namespace MyAcc.Recorder {
  public static class StreamReaderExtensions {
    public static string ReadAccString( this BinaryReader reader ) {
      var length = reader.ReadUInt16();
      var bytes = reader.ReadBytes( length );
      return Encoding.UTF8.GetString( bytes );
    }
  }
}