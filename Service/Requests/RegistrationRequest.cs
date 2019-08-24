using System;
using System.Collections.Generic;
using Service.Fields;

namespace Service.Requests {
  internal class RegistrationRequest : AccApiRequest {
    private readonly string _password;
    private readonly TimeSpan? _updateInterval;
    private readonly TimeSpan _defaultUpdateInterval = TimeSpan.FromMilliseconds( 250 );

    public RegistrationRequest( string password, TimeSpan? updateInterval ) {
      _password = password;
      _updateInterval = updateInterval;
    }

    public override IEnumerable<IApiField> GetFields() {
      var updateIntervalMilliseconds = _updateInterval
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