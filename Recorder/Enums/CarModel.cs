using System.ComponentModel;

namespace MyAcc.Recorder.Enums {
  internal enum CarModel: byte {
    [Description("Porsche 991 GT3 R")]
    Porsche991,
    [Description("Mercedes-AMG GT3")]
    MercedesAmgGT3,
    [Description("Ferrari 488 GT3")]
    Ferrari488,
    [Description("Audi R8 LMS")]
    AudiR8,
    [Description("Lamborghini Huracan GT3")] 
    LamborghiniHuracan,
    [Description("McLaren 650S GT3")]    
    McLaren650S,
    [Description("Nissan GT-R Nismo GT3")]    
    NissanGtr,
    [Description("BMW M6 GT3")]    
    BmwM6,
    [Description("Bentley Continental GT3")]    
    BentleyContinental,
    [Description( "Porsche 991 II GT3 Cup" )]
    Porsche991IICup,
    [Description( "Nissan GT-R Nismo GT3" )]
    NissanGtrAlt,
    L,
    [Description("AMR V12 Vantage GT3")] 
    AmrVantageV12,
    [Description( "Reiter Engineering R-EX GT3" )] 
    ReiterREX,
    O,
    [Description("Lexus RC F GT3")]
    LexusRCF,
    Q,
    [Description("Honda NSX GT3")]
    HondaNsx,
    [Description("Lamborghini Huracan ST")]
    LamborghiniHuracanST
  }
}