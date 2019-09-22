using MyAcc.Recorder.Enums;

namespace MyAcc.Recorder.Responses {
  internal class DriverInfo {
    public DriverInfo( string firstName, string lastName, string shortName, DriverCategory category ) {
      FirstName = firstName;
      LastName = lastName;
      ShortName = shortName;
      Category = category;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string ShortName { get; }
    public DriverCategory Category { get; }
  }
}