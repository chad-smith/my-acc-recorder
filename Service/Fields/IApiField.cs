using System.IO;

namespace Service.Requests {
  internal interface IApiField {
    void WriteValue( BinaryWriter writer );
  }
}