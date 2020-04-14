namespace CondorDotNet.Model {
  /// <summary>
  /// Represents a point on a polar chart.
  /// </summary>
  public class Point {
    private const double MPSToKMph = 3.6;

    /// <summary>
    /// The glide ratio in zero wind.
    /// </summary>
    public double GlideRatio => GetGlideRatio(0);

    /// <summary>
    /// The sink rate (m/s).
    /// </summary>
    public double Sink { get; set; }

    /// <summary>
    /// The speed in (km/h).
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// Returns the glide ratio calculated for the specified wind speed.
    /// </summary>
    /// <param name="wind">The wind speed in km/h. Tailwinds are positive.</param>
    public double GetGlideRatio(double wind) {
      return -(Speed + wind) / (MPSToKMph * Sink);
    }

    /// <summary>
    /// Returns a <see cref="System.Drawing.PointF"/> containing the current values.
    /// </summary>
    /// <returns></returns>
    public System.Drawing.PointF ToPointF() {
      return new System.Drawing.PointF((float)Speed, (float)Sink);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() {
      return $"{Speed:N0} km/h, {Sink:0.00} m/s, {GlideRatio:0.0}:1";
    }
  }
}
