using System.Collections.Generic;
using System.IO;

namespace Service.Responses {
  internal class EntryListCarResponse: AccApiResponse {
    public EntryListCarResponse( BinaryReader reader ) {
      Drivers = new List<DriverInfo>();

      CarId = reader.ReadUInt16();

/*
      var carInfo = _entryListCars.SingleOrDefault( x => x.CarIndex == carId );
      if ( carInfo == null ) {
        System.Diagnostics.Debug.WriteLine( $"Entry list update for unknown carIndex {carId}" );
        break;
      }
*/

      CarModel = (CarModel)reader.ReadByte();
      TeamName = reader.ReadAccString();
      RaceNumber = reader.ReadInt32();
      CupCategory = (CupCategory)reader.ReadByte();
      CurrentDriverIndex = reader.ReadByte();

      // Now the drivers on this car:
      var numberOfDriversForCar = reader.ReadByte();
      for ( var driverIndex = 0; driverIndex < numberOfDriversForCar; driverIndex++ ) {
        var driverInfo = new DriverInfo(
          reader.ReadAccString(),
          reader.ReadAccString(),
          reader.ReadAccString(),
          (DriverCategory)reader.ReadByte()
        );

        Drivers.Add( driverInfo );
      }
    }

    public override string ToString() {
      return $"{CarId} {CarModel} {RaceNumber} {TeamName}";
    }

    public byte CurrentDriverIndex { get; }

    public CupCategory CupCategory { get; }
    public int RaceNumber { get; }
    public string TeamName { get; }
    public List<DriverInfo> Drivers { get; }
    public CarModel CarModel { get; }
    public ushort CarId { get; }
  }

}