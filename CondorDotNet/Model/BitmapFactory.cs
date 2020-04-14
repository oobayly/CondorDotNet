using CondorDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace CondorDotNet.Model {
  /// <summary>
  /// A factory for generating polar bitmaps.
  /// </summary>
  public class BitmapFactory {
    #region Fields
    private readonly Color[] PolarColors = {
      Color.Blue,
      Color.Red,
      Color.DarkGreen
    };

    private const int DefaultSize = 2048;

    private const double DefaultMaxSpeed = 200;

    private const int Margin = 20;
    #endregion

    #region Properties
    /// <summary>
    /// The aircraft for which the polar should be generated.
    /// </summary>
    public Aircraft Aircraft { get; set; }

    private Rectangle DrawableBounds => new Rectangle(Margin, Margin, Width - Margin * 2, Height - Margin * 2);

    private Font Font => new Font("Arial", 12);

    /// <summary>
    /// The height of the bitmap (px).
    /// </summary>
    public int Height { get; set; } = DefaultSize;

    /// <summary>
    /// The list of masses for which polars should be displayed.
    /// </summary>
    public IEnumerable<double> Mass { get; set; }

    /// <summary>
    /// The maximum speed to be show on the polot (km/h).
    /// </summary>
    public double MaxSpeed { get; set; } = DefaultMaxSpeed;

    private double StepSink => Units == UnitType.Imperial ? .5 : .25;

    private double StepSpeed => Units == UnitType.Metric ? 20 : 10;

    /// <summary>
    /// The units to be used on the plot.
    /// </summary>
    public UnitType Units { get; set; } = UnitType.Metric;

    /// <summary>
    /// The width of the bitmap (px).
    /// </summary>
    public int Width { get; set; } = DefaultSize;
    #endregion

    #region Methods
    private PointF DrawGrid(Graphics g, double maxSpeed, double minSink) {
      // Maxima in display units
      var displaySpeed = Math.Ceiling(maxSpeed.ConvertSpeed(Units) / StepSpeed) * StepSpeed;
      var displaySink = Math.Floor(minSink.ConvertSinkRate(Units) / StepSink) * StepSink;

      // Maxima in native units (metric)
      var speed = displaySpeed.ConvertSpeed(UnitType.Metric, Units);
      var sink = displaySink.ConvertSinkRate(UnitType.Metric, Units);

      var bounds = DrawableBounds;
      var scaleX = bounds.Width / (float)speed;
      var scaleY = bounds.Height / (float)sink;
      var pen = new Pen(Brushes.LightGray, 0) {
        DashStyle = DashStyle.Dash,
      };

      g.TranslateTransform(Margin, Margin);

      // Bounds
      g.DrawRectangle(Pens.Black, 0, 0, bounds.Width, bounds.Height);

      // Grid
      for (var i = StepSpeed; i < displaySpeed; i+= StepSpeed) {
        var label = i.ToSpeedString(Units);
        var width = g.MeasureString(label, Font).Width;
        var x = (float)(i.ConvertSpeed(UnitType.Metric, Units) * scaleX);
        var y = (float)(sink * scaleY);

        g.DrawLine(pen, x, 0, x, y);
        g.DrawString(label, Font, Brushes.Black, x - (width / 2), 0 + 5);
      }

      for (var j = -StepSink; j > displaySink; j -= StepSink) {
        var label = ((double)j).ToSinkString(Units);
        var x = (float)(speed * scaleX);
        var y = (float)(j.ConvertSinkRate(UnitType.Metric, Units) * scaleY);

        g.DrawLine(pen, 0, y, x, y);
        g.DrawString(label, Font, Brushes.Black, 5, y - (Font.Height / 2));
      }


      g.ResetTransform();

      return new PointF(scaleX, scaleY);
    }

    /// <summary>
    /// Returns a GDI+ <see cref="Bitmap"/> containing the polar for the specified <see cref="Model.Aircraft"/>.
    /// </summary>
    public Bitmap Generate() {
      var mass = (Mass?.ToArray() ?? new double[] { Aircraft.MassReference });
      var polars = mass
        .Select(x => Aircraft.Polar.ShiftPolar(Aircraft.MassReference, x))
        .ToArray();
      var maxSpeed = Math.Min(MaxSpeed, polars.Min(x => x.Last().Point.Speed));
      var minSink = polars.Min(x => x.Get(maxSpeed).Sink);

      var image = new Bitmap(Width, Height);

      using (var g = Graphics.FromImage(image)) {
        g.Clear(Color.White);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;

        var bounds = DrawableBounds;
        var scale = DrawGrid(g, maxSpeed, minSink);

        g.Clip = new Region(bounds);

        // Polars
        g.TranslateTransform(bounds.X, bounds.Y);
        g.ScaleTransform(scale.X, scale.Y);

        for (var i = 0; i < polars.Length; i++) {
          var pen = new Pen(PolarColors[i], 0);

          g.DrawPath(pen, polars[i].GetGdiPath());
        }

        g.ResetTransform();

        // Title
        using (var legendFont = new Font(Font.FontFamily, 30, FontStyle.Regular)) {
          g.TranslateTransform(200, Width - Margin - (1.2f * (1 + polars.Length) * legendFont.Height));

          using (var titleFont = new Font(Font.FontFamily, 30, FontStyle.Bold)) {
            g.DrawString($"{Aircraft.Name}", titleFont, Brushes.Black, 0, 0);
          }

          for (var i = 0; i < polars.Length; i++) {
            var color = PolarColors[i];
            var y = 1.2f * (1 + i) * legendFont.Height;
            var massText = mass[i].ConvertMass(Units).ToMassString(Units);
            var wingLoadingText = Aircraft.GetWingLoading(mass[i]).ConvertWingLoading(Units).ToWingLoadingString(Units);

            g.DrawString($"{massText}\t~ {wingLoadingText}", legendFont, new SolidBrush(color), 0, y);
          }
        }

        g.Flush();
      }

      return image;
    }

    #endregion
  }
}
