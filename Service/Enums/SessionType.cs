using System.ComponentModel;

namespace MyAcc.Recorder.Enums {
  public enum SessionType {
    Practice = 0,
    Qualifying = 4,
    Superpole = 9,
    Race = 10,
    Hotlap = 11,
    Hotstint = 12,
    [Description("Hotlap Superpole")]
    HotlapSuperpole = 13,
    Replay = 14
  };
}