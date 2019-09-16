using Service.Responses;

namespace Service {
  internal class CarDetails {
    public int Id { get; private set; }
    public CarModel CarModel { get; private set; }
    public string CurrentDriver { get; private set; }
    public bool Disconnected { get; private set; }
    public int? Position { get; set; }
    public CarLocation Location { get; set; }
    public int LapCount { get; set; }
    public int Delta { get; set; }
    public LapInfo CurrentLap { get; set; }
    public LapInfo LastLap { get; set; }

    public CarDetails( int carId, CarModel carModel, string currentDriver ) {
      Id = carId;
      CarModel = carModel;
      CurrentDriver = currentDriver;
      Disconnected = false;
    }

    public void Disconnect() {
      Position = null;
      Disconnected = true;
    }

    public void UpdateBasicDetails( CarModel carModel, string driverName ) {
      CarModel = carModel;
      CurrentDriver = driverName;
      Disconnected = false;
    }
  }
}