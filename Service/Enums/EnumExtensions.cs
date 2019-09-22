using System;
using System.ComponentModel;
using System.Linq;

namespace MyAcc.Recorder.Enums {
  internal static class EnumExtensions {
    public static string GetDescription( this Enum e ) {
      var genericEnumType = e.GetType();
      var memberInfo = genericEnumType.GetMember( e.ToString() );

      if ( ( memberInfo.Length <= 0 ) ) {
        return e.ToString();
      }

      var attributes = memberInfo[0].GetCustomAttributes( typeof(DescriptionAttribute), false );

      if ( attributes.Length > 0 ) {
        return ( (DescriptionAttribute)attributes.ElementAt( 0 ) ).Description;
      }

      return e.ToString();
    }
  }
}