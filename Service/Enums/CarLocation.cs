using System.ComponentModel;

namespace MyAcc.Recorder.Enums
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
