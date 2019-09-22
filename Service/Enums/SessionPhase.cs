using System.ComponentModel;

namespace MyAcc.Recorder.Enums {
  public enum SessionPhase {
    None = 0,
    Starting = 1,
    [Description( "Pre-formation" )]
    PreFormation = 2,
    [Description( "Formation Lap" )]
    FormationLap = 3,
    [Description( "Pre-session" )]
    PreSession = 4,
    Session = 5,
    [Description( "Session Over" )]
    SessionOver = 6,
    [Description( "Post-session" )]
    PostSession = 7,
    [Description( "Finished" )]
    ResultUI = 8
  };
}