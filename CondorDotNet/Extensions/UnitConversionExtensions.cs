using CondorDotNet.Model;

namespace CondorDotNet.Extensions {
  /// <summary>
  /// A collection of utilities to help unit conversions.
  /// </summary>
  public static class UnitConversionExtensions {
    private const double InchToCM = 2.54;

    private const double NMToKm = 1.852;

    private const double KnotToMPS = 1852.0 / 3600.0;

    private const double LbToKg = 0.4536;

    private const double GallToLitre = 231 * InchToCM * InchToCM * InchToCM / 1000; // 231 cubic inches

    /// <summary>
    /// Returns the current value converted from <paramref name="from"/> units to <paramref name="to"/> mass units.
    /// </summary>
    /// <param name="value">The current value.</param>
    /// <param name="to">The units to convert to.</param>
    /// <param name="from">The units to convert from.</param>
    public static double ConvertMass(this double value, UnitType to, UnitType from = UnitType.Metric) {
      if (to == from) {
        return value;
      }

      if (from == UnitType.Imperial) {
        value *= LbToKg;
      }

      if (to == UnitType.Imperial) {
        value /= LbToKg;
      }

      return value;
    }

    /// <summary>
    /// Returns the current value converted from <paramref name="from"/> units to <paramref name="to"/> sink-rate units.
    /// </summary>
    /// <param name="value">The current value.</param>
    /// <param name="to">The units to convert to.</param>
    /// <param name="from">The units to convert from.</param>
    public static double ConvertSinkRate(this double value, UnitType to, UnitType from = UnitType.Metric) {
      if (to == from) {
        return value;
      }

      if (from == UnitType.Imperial) {
        value *= KnotToMPS;
      }

      if (to == UnitType.Imperial) {
        value /= KnotToMPS;
      }

      return value;
    }

    /// <summary>
    /// Returns the current value converted from <paramref name="from"/> units to <paramref name="to"/> speed units.
    /// </summary>
    /// <param name="value">The current value.</param>
    /// <param name="to">The units to convert to.</param>
    /// <param name="from">The units to convert from.</param>
    public static double ConvertSpeed(this double value, UnitType to, UnitType from = UnitType.Metric) {
      if (to == from) {
        return value;
      }

      if (from != UnitType.Metric) {
        value *= NMToKm;
      }

      if (to != UnitType.Metric) {
        value /= NMToKm;
      }

      return value;
    }

    /// <summary>
    /// Returns the current value converted from <paramref name="from"/> units to <paramref name="to"/> mass units.
    /// </summary>
    /// <param name="value">The current value.</param>
    /// <param name="to">The units to convert to.</param>
    /// <param name="from">The units to convert from.</param>
    public static double ConvertVolume(this double value, UnitType to, UnitType from = UnitType.Metric) {
      if (to == from) {
        return value;
      }

      if (from == UnitType.Imperial) {
        value *= GallToLitre;
      }

      if (to == UnitType.Imperial) {
        value /= GallToLitre;
      }

      return value;
    }
  }
}
