using CondorDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace CondorDotNet.Model {
  /// <summary>
  /// Represents a glider polar plot. 
  /// </summary>
  public class Polar : LinkedList<ControlPoint> {
    #region Fields
    /// <summary>
    /// The number of steps used for caclulating MacCready speed to fly. 
    /// </summary>
    private const int MCSteps = 1000;
    #endregion

    #region Methods
    /// <summary>
    /// Gets the values for the specified speed (km/h).
    /// </summary>
    /// <param name="speed">The speed (km/h).</param>
    /// <returns></returns>
    public Point Get(double speed) {
      Point last = null;

      var output = this.Interpolate(p => {
        if (speed >= last?.Speed && speed <= p.Speed) {
          return true;
        }

        last = p;

        return false;
      });

      if (output == null) {
        throw new InvalidOperationException($"No value could be found on the polar at {speed:0} km/h.");
      }

      return last.Interpolate(output, .5);
    }

    /// <summary>
    /// Gets the best glide ratio.
    /// </summary>
    public Point GetBestGlide() {
      return GetMC(0, 0);
    }

    /// <summary>
    /// Gets the speed-to-fly for the given MacCready number and wind speed.
    /// </summary>
    /// <param name="mc">The MacCready number in m/s.</param>
    /// <param name="wind">The wind speed in km/h. Tailwinds are positive.</param>
    /// <returns>The point on the polar that gives the speed-to-fly.</returns>
    public Point GetMC(double mc, double wind) {
      GetMC(mc, wind, out Point point);

      return point;
    }

    /// <summary>
    /// Gets the speed-to-fly for the given MacCready number and wind speed.
    /// </summary>
    /// <param name="mc">The MacCready number in m/s.</param>
    /// <param name="wind">The wind speed in km/h. Tailwinds are positive.</param>
    /// <param name="point">The point on the polar that gives the speed-to-fly.</param>
    /// <returns>The glide ratio.</returns>
    public double GetMC(double mc, double wind, out Point point) {
      var origin = new Point() {
        Speed = -wind,
        Sink = mc
      };
      var lastSlope = double.MinValue; // This is not a glide ration, but the slope of the chart
      Point lastPoint = null;

      var output = this.Interpolate(p => {
        var slope = p.GetPolarSlope(origin);

        if (slope < lastSlope) {
          return true;
        }

        lastSlope = slope;
        lastPoint = p;

        return false;
      });

      if (output == null) {
        throw new InvalidOperationException("No speed-to-fly value could be found.");
      }

      point = lastPoint;

      return point.GetGlideRatio(wind);
    }

    /// <summary>
    /// Gets the minimum sink rate.
    /// </summary>
    public Point GetMinimumSink() {
      Point lastPoint = null;

      var output = this.Interpolate(p => {
        if (p.Sink < lastPoint?.Sink) {
          return true;
        }

        lastPoint = p;

        return false;
      });

      if (output == null) {
        throw new InvalidOperationException("No minimum sink rate value could be found.");
      }
      return lastPoint;
    }

    /// <summary>
    /// Gets the minimum speed.
    /// </summary>
    public Point GetMinimumSpeed() {
      return First.Value.Point;
    }

    /// <summary>
    /// Returns a new <see cref="Polar"/> instance that has been shifted due to a change in
    /// mass.
    /// </summary>
    /// <param name="refMass">The reference mass (kg).</param>
    /// <param name="newMass">The new mass in (kg).</param>
    public Polar ShiftPolar(double refMass, double newMass) {
      var multiplier = Math.Sqrt(newMass / refMass);

      var clone = new Polar();

      var currentNode = First;

      do {
        clone.AddLast(currentNode.Value.Shift(multiplier));

        currentNode = currentNode.Next;
      } while (currentNode != null);

      return clone;
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Creates an instance of <see cref="Polar"/> class from the specified Condor pol file.
    /// </summary>
    /// <param name="file">The file from which the Polar2 should be loaded.</param>
    public static Polar Load(FileInfo file) {
      return Load(file.FullName);
    }

    /// <summary>
    /// Creates an instance of <see cref="Polar"/> class from the specified Condor pol file.
    /// </summary>
    /// <param name="file">The file from which the Polar2 should be loaded.</param>
    public static Polar Load(string file) {
      using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
        return Load(fs);
      }
    }

    /// <summary>
    /// Creates an instance of <see cref="Polar"/> class from the specified Condor pol file stream.
    /// </summary>
    /// <param name="stream">The stream from which the Polar2 should be loaded.</param>
    public static Polar Load(Stream stream) {
      var polar = new Polar();

      using (var reader = new BinaryReader(stream)) {
        var count = reader.ReadInt32();
        var points = new LinkedList<ControlPoint>();

        for (var i = 0; i < count; i++) {
          polar.AddLast(reader.ReadControlPoint());
        }
      }

      return polar;
    }
    #endregion
  }
}
