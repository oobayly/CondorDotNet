using CondorDotNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CondorDotNet.Extensions {
  internal static class Extensions {
    #region Fields
    /// <summary>
    /// The number of steps used for caclulating MacCready speed to fly. 
    /// </summary>
    private const int MCSteps = 1000;
    #endregion

    #region Methods
    /// <summary>
    /// Returns the slope (on the polar plot, not a glide ration) from the specified origin to the current <see cref="Point"/>.
    /// </summary>
    internal static double GetPolarSlope(this Point value, Point origin) {
      return (value.Sink - origin.Sink) / (value.Speed - origin.Speed);
    }

    /// <summary>
    /// Gets the point interpolated distance <paramref name="t"/> along the bezier curve
    /// specified by the <paramref name="values"/> control points.
    /// </summary>
    /// <param name="values">The bezier points.</param>
    /// <param name="t">The distance along the curve, between 0 and 1.</param>
    internal static Point[] InterpolateBezier(this Point[] values, double t) {
      var resp = new Point[values.Length - 1];

      for (var i = 0; i < resp.Length; i++) {
        resp[i] = values[i].Interpolate(values[i + 1], t);
      }

      if (resp.Length != 1) {
        resp = InterpolateBezier(resp, t);
      }

      return resp;
    }

    /// <summary>
    /// Gets the point interpolated linearly distance <paramref name="t"/> between
    /// the current <see cref="Point"/> and the next <see cref="Point"/>.
    /// </summary>
    /// <param name="value">The current point.</param>
    /// <param name="next">The next point.</param>
    /// <param name="t">The interpolation distance, between 0 and 1.</param>
    /// <returns></returns>
    internal static Point Interpolate(this Point value, Point next, double t) {
      return new Point() {
        Sink = value.Sink + (t * (next.Sink - value.Sink)),
        Speed = value.Speed + (t * (next.Speed - value.Speed)),
      };
    }

    /// <summary>
    /// Interpolates the specified <see cref="LinkedList{ControlPoint}"/>.
    /// </summary>
    /// <param name="list">The list of <see cref="ControlPoint"/> objects.</param>
    /// <param name="func">The callback used to provide the interpolated <see cref="Point"/>. Returning true will stop interpolation.</param>
    /// <param name="forward">True if interpolating forward, otherwise interpolate backwards.</param>
    /// <param name="steps">The number of steps to use when interpolating a bezier curve.</param>
    /// <returns>The point retuned as indicated by the <paramref name="func"/> callback.</returns>
    internal static Point Interpolate(this LinkedList<ControlPoint> list, Func<Point, bool> func, bool forward = true, int steps = MCSteps) {
      var current = forward ? list.First : list.Last;

      do {
        for (var j = 0; j <= 1000; j++) {
          var t = (double)j / steps;
          var p = current.Interpolate(t);

          if (func(p)) {
            return p;
          }
        }

        if (forward) {
          if ((current = current.Next).Next == null) {
            break;
          }
        } else {
          if ((current = current.Previous).Previous == null) {
            break;
          }
        }
      } while (true);

      return null;
    }

    /// <summary>
    /// Gets the point interpolated between this 
    /// </summary>
    /// <param name="value">The current node in a <see cref="LinkedList{ControlPoint}"/>.</param>
    /// <param name="t">The distance along the curve, between 0 and 1.</param>
    /// <param name="forward">True if interpolating forward, otherwise interpolate backwards.</param>
    /// <returns></returns>
    internal static Point Interpolate(this LinkedListNode<ControlPoint> value, double t, bool forward = true) {
      if (t < 0 || t > 1) {
        throw new ArgumentOutOfRangeException("The value must be between 0 and 1.", nameof(t));
      }

      Point[] points;

      if (forward) {
        points = new Point[] { value.Value.Point, value.Value.NextHandle, value.Next.Value.PreviousHandle, value.Next.Value.Point };
      } else {
        points = new Point[] { value.Value.Point, value.Value.PreviousHandle, value.Previous.Value.NextHandle, value.Previous.Value.Point };
      }

      return points.InterpolateBezier(t).First();
    }

    internal static ControlPoint ReadControlPoint(this BinaryReader reader) => new ControlPoint {
      Point = reader.ReadPoint(),
      PreviousHandle = reader.ReadPoint(),
      NextHandle = reader.ReadPoint()
    };

    internal static Point ReadPoint(this BinaryReader reader) {
      return new Point() {
        Speed = reader.ReadSingle(),
        Sink = reader.ReadSingle()
      };
    }

    internal static Point Shift(this Point value, double speedMultiplier) {
      return new Point() {
        Speed = value.Speed * speedMultiplier,
        Sink = value.Sink * speedMultiplier
      };
    }

    internal static ControlPoint Shift(this ControlPoint value, double speedMultiplier) {
      return new ControlPoint() {
        NextHandle = value.NextHandle.Shift(speedMultiplier),
        Point = value.Point.Shift(speedMultiplier),
        PreviousHandle = value.PreviousHandle.Shift(speedMultiplier)
      };
    }
    #endregion
  }
}
