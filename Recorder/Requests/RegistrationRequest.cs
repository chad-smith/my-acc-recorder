using System;
using System.Collections.Generic;
using MyAcc.Recorder.Fields;

namespace MyAcc.Recorder.Requests {
  internal class RegistrationRequest : AccApiRequest {
    private readonly string _password;
    public TimeSpan? UpdateInterval { get; }
    private readonly TimeSpan _defaultUpdateInterval = TimeSpan.FromSeconds( 1 );

    public RegistrationRequest( string password, TimeSpan? updateInterval ) {
      _password = password;
      UpdateInterval = updateInterval;
    }

    public override IEnumerable<IApiField> GetFields() {
      var updateIntervalMilliseconds = UpdateInterval
        .GetValueOrDefault( _defaultUpdateInterval )
        .TotalMilliseconds;

      return new IApiField[] {
        new ByteField( 1 ),
        new ByteField( 3 ),
        new StringField( "ACC Results Publisher" ),
        new StringField( _password ),
        new IntField( Convert.ToInt32( updateIntervalMilliseconds ) ),
        new StringField( "" )
      };
    }
  }
}