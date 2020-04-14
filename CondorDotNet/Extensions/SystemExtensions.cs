using CondorDotNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace System {
  internal static  class SystemExtensions {
    internal static T ParseValue<T>(this string value, string units) {
      value = value.Replace($" {units}", "");

      return (T)Convert.ChangeType(value, typeof(T));
    }

    internal static T ParseValue<T>(this IEnumerable<string[]> value, string match, string units, bool optional = false) {
      return value.ParseValue<T>((x) => {
        return string.Compare(match, x, true) == 0;
      }, units, optional);
    }

    internal static T ParseValue<T>(this IEnumerable<string[]> value, Func<string, bool> predicate, string units, bool optional = false) {
      var found = value.FirstOrDefault(x => predicate.Invoke(x.First()));

      if (found == null) {
        if (optional) {
          return default;
        } else {
          throw new InvalidOperationException("No matching item could be found.");
        }
      }

      return found[1].ParseValue<T>(units);
    }

    internal static string ToMassString(this double value, UnitType units) {
      var unitText = units == UnitType.Imperial ? "lb" : "kg";

      return $"{value:N0} {unitText}";
    }

    internal static string ToSinkString(this double value, UnitType units) {
      var unitText = units == UnitType.Imperial ? "Kts" : "m/s";

      return $"{value:0.00} {unitText}";
    }

    internal static string ToSpeedString(this double value, UnitType units) {
      var unitText = units == UnitType.Metric ? "km/h" : "Kts";

      return $"{value:0} {unitText}";
    }

    internal static string ToWingLoadingString(this double value, UnitType units) {
      var unitText = units == UnitType.Metric ? "kg/m²" : "lb/ft²";

      return $"{value:0.0} {unitText}";
    }
  }
}
