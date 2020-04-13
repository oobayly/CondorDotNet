using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CondorDotNet.Model {
  /// <summary>
  /// Represents an aircraft object.
  /// </summary>
  public class Aircraft {
    #region Fields
    /// <summary>
    /// The assumed standard mass of a pilot (in kg).
    /// </summary>
    private const double MassPilot = 80;

    /// <summary>
    /// The minimum mass of a pilot (in kg). Used for <see cref="WingLoadingMin"/>.
    /// </summary>
    private const double MassPilotMin = 70;
    #endregion

    #region Properties
    /// <summary>
    /// The DAec handicap index.
    /// </summary>
    public int DAeCIndex { get; set; }

    /// <summary>
    /// The length of the aircraft in (metre).
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// The name of the aircraft type.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The best glide ratio.
    /// </summary>
    public double GlideRatioBest => Polar.GetBestGlide().GlideRatio;

    /// <summary>
    /// The mass of the aircraft when empty (kg).
    /// </summary>
    public double MassEmpty { get; set; }

    /// <summary>
    /// The reference mass of the aircraft used for calculating the <see cref="Polar"/> (kg).
    /// </summary>
    public double MassReference => MassEmpty + MassPilot;

    /// <summary>
    /// The maximum mass of the aircraft (kg).
    /// </summary>
    public double MassMax { get; set; }

    /// <summary>
    /// The aircraft's polar data.
    /// </summary>
    public Polar Polar { get; set; }

    /// <summary>
    /// The minimum sink rate of the aircraft (m/s).
    /// </summary>
    public double SinkMin => Polar.GetMinimumSink().Sink;

    /// <summary>
    /// The maneuvering speed of the aircraft (km/h).
    /// </summary>
    public double SpeedManeuvering { get; set; }

    /// <summary>
    /// The best-glide speed of the aircraft (km/h).
    /// </summary>
    public double SpeedBestGlide => Polar.GetBestGlide().Speed;

    /// <summary>
    /// The maximum speed (Vne) of the aircraft (km/h).
    /// </summary>
    public double SpeedMax { get; set; }

    /// <summary>
    /// The minimum speed of the aircraft (km/h).
    /// </summary>
    public double SpeedMin => Polar.GetMinimumSpeed().Speed;

    /// <summary>
    /// The volume of water the can be carried as ballast (litre).
    /// </summary>
    public double WaterBallast { get; set; }

    /// <summary>
    /// The wing area of the aircraft (metre²).
    /// </summary>
    public double WingArea { get; set; }

    /// <summary>
    /// The maximum wing loading of the aircraft (kg/m²).
    /// </summary>
    public double WingLoadingMax => (MassMax) / WingArea;

    /// <summary>
    /// The minimum wing loading of the aircraft (kg/m²).
    /// </summary>
    public double WingLoadingMin  => (MassEmpty + MassPilotMin) / WingArea;

    /// <summary>
    /// The wing span of the aircraft (metre).
    /// </summary>
    public double WingSpan { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Returns a string containing the WinPilot Polar.
    /// See: http://www.winpilot.com/polar.asp
    /// See: https://www.cumulus-soaring.com/polars.htm
    /// <param name="mass">The optional mass of the airaft (kg).</param>
    /// </summary>
    public string ToWinPolar(double? mass = null) {
      var polar = Polar;

      if (mass == null) {
        mass = MassReference;
      } else {
        polar = Polar.ShiftPolar(MassReference, mass.Value);
      }

      var speed3 = Math.Min(SpeedMax, polar.Last.Value.Point.Speed); // Don't bother with speeds above Vne.
      var p1 = polar.GetMinimumSink();
      var p3 = polar.Get(speed3);
      var p2 = polar.Get((p1.Speed + p3.Speed) / 2);

      return $@"*{Name} WinPilot POLAR file: Created by {Assembly.GetExecutingAssembly()}
*MassDryGross[kg], MaxWaterBallast[liters], Speed1[km/h], Sink1[m/s], Speed2, Sink2, Speed3, Sink3
{mass}, {WaterBallast}, {p1.Speed:0.00}, {p1.Sink:0.00}, {p2.Speed:0.00}, {p2.Sink:0.00}, {p3.Speed:0.00}, {p3.Sink:0.00}";
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() {
      return Name ?? "";
    }

    /// <summary>
    /// Writes the WinPilot polar to the specified file.
    /// </summary>
    /// <param name="file">The file to write to.</param>
    /// <param name="mass">The optional mass of the airaft (kg).</param>
    public void WriteWinPolar(FileInfo file, double? mass = null) {
      WriteWinPolar(file.FullName, mass);
    }

    /// <summary>
    /// Writes the WinPilot polar to the specified file.
    /// </summary>
    /// <param name="file">The name of the file to write to.</param>
    /// <param name="mass">The optional mass of the airaft (kg).</param>
    public void WriteWinPolar(string file, double? mass = null) {
      using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None)) {
        WriteWinPolar(fs, mass);
      }
    }

    /// <summary>
    /// Writes the WinPilot polar to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="mass">The optional mass of the airaft (kg).</param>
    public void WriteWinPolar(Stream stream, double? mass = null) {
      using (var writer = new StreamWriter(stream)) {
        writer.Write(ToWinPolar(mass));
      }
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Creates and instance of the <see cref="Aircraft"/> class from the specified directory.
    /// </summary>
    public static Aircraft Load(string directory) {
      return Load(new DirectoryInfo(directory));
    }

    /// <summary>
    /// Creates and instance of the <see cref="Aircraft"/> class from the specified directory.
    /// </summary>
    public static Aircraft Load(DirectoryInfo directory) {
      var polarFile = Path.Combine(directory.FullName, $"{directory.Name}.pol");
      var dataFile = Path.Combine(directory.FullName, $"{directory.Name}.txt");
      Aircraft aircraft;

      using (var fs = new FileStream(dataFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
        aircraft = Load(fs);
      }

      aircraft.Polar = Polar.Load(polarFile);

      return aircraft;
    }

    private static Aircraft Load(Stream stream) {
      var lines = new List<string[]>();

      using (var reader = new StreamReader(stream)) {
        while (true) {
          var line = reader.ReadLine();

          if (line == null) {
            break;
          }

          line = line.Trim();

          if (line != "") {
            lines.Add(line.Split(',').Select(x => x.Trim()).ToArray());
          }
        }
      }

      var aircraft = new Aircraft() {
        Name = lines.First().First()
      };

      // Dimensions
      aircraft.WingArea = lines.ParseValue<double>("Wing area", "m2");
      aircraft.WingSpan = lines.ParseValue<double>("Wing span", "m");
      aircraft.Length = lines.ParseValue<double>("Length", "m");

      // Mass
      aircraft.MassEmpty = lines.ParseValue<double>(x => x == "Empty weight" || x == "Empty mass", "kg");
      aircraft.MassMax = lines.ParseValue<double>(x => x == "Max weight" || x == "Max mass", "kg");
      aircraft.WaterBallast = lines.ParseValue<double>("Water ballast", "l", true);

      // Speeds
      aircraft.SpeedManeuvering = lines.ParseValue<double>("Maneuvering speed", "km/h", true);
      aircraft.SpeedMax = lines.ParseValue<double>("Max speed", "km/h");

      // Other 
      aircraft.DAeCIndex = lines.ParseValue<int>("DAeC index", "");

      return aircraft;
    }
    #endregion
  }
}
