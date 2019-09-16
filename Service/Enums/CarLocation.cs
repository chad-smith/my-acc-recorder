using System.ComponentModel;

namespace Service
{
  public enum CarLocation {
    None = 0,
    Track = 1,
    Pitlane = 2,
    [Description("Pit Entry")]
    PitEntry = 3,
    [Description( "Pit Exit" )]
    PitExit = 4
  }
}
