using System.IO;

namespace Service.Fields {
  internal interface IApiField {
    void WriteValue( BinaryWriter writer );
  }
}