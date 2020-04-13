namespace CondorDotNet.Model {
  /// <summary>
  /// The list of allowed values for unit conversion.
  /// </summary>
  public enum UnitType {
    /// <summary>
    /// Full metric system - km/h, m/s, kg, litres.
    /// </summary>
    Metric,
    /// <summary>
    /// Fully imperial - knots, knots, lbs, US gal.
    /// </summary>
    Imperial,
    /// <summary>
    /// Australian - knots, m/s, kg, litres.
    /// </summary>
    Australian
  }
}
