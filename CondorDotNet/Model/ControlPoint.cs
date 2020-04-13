using System;

namespace CondorDotNet.Model {
  /// <summary>
  /// Represents a control point on a polar chart.
  /// </summary>
  public class ControlPoint {
    #region Properties
    /// <summary>
    /// The <see cref="Point"/> that specifies the next handle of the bezier curve.
    /// </summary>
    public Point NextHandle { get; set; }

    /// <summary>
    /// The <see cref="Point"/> that specifies the start or end of the bezier curve.
    /// </summary>
    public Point Point { get; set; }

    /// <summary>
    /// The <see cref="Point"/> that specifies the previous handle of the bezier curve.
    /// </summary>
    public Point PreviousHandle { get; set; }
    #endregion
  }
}
