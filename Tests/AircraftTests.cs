using CondorDotNet.Model;
using Newtonsoft.Json;
using System.IO;
using Tests.Extensions;
using Tests.Model;
using Xunit;

namespace Tests {
  public class AircraftTests : ICondorTest {
    private string PlanesPath => this.GetPlanesPath();

    private string ASK21Path => Path.Combine(PlanesPath, "ASK21");

    [Fact]
    public void LoadAircraftTest() {
      var aircraft = Aircraft.Load(ASK21Path);

      Assert.Equal("Alexander Schleicher ASK21", aircraft.Name);
      Assert.Equal("Alexander Schleicher ASK21", aircraft.ToString());
      Assert.Equal(17, aircraft.WingSpan);
      Assert.Equal(17.95, aircraft.WingArea);
      Assert.Equal(8.35, aircraft.Length);
      Assert.Equal(360, aircraft.MassEmpty);
      Assert.Equal(440, aircraft.MassReference);
      Assert.Equal(600, aircraft.MassMax);
      Assert.Equal(0, aircraft.WaterBallast);
      Assert.Equal(24.0, aircraft.WingLoadingMin, 1);
      Assert.Equal(33.4, aircraft.WingLoadingMax, 1);
      Assert.Equal(280, aircraft.SpeedMax);
      Assert.Equal(180, aircraft.SpeedManeuvering);
      Assert.Equal(63, aircraft.SpeedMin, 0);
      Assert.Equal(-0.65, aircraft.SinkMin, 2);
      Assert.Equal(33.4, aircraft.GlideRatioBest, 1);
      Assert.Equal(87, aircraft.SpeedBestGlide, 0);
      Assert.Equal(92, aircraft.DAeCIndex);
      Assert.Equal(5, aircraft.Polar.Count);

      aircraft.ToWinPolar(aircraft.MassMax);
    }

    [Fact]
    public void LoadAllAircraftTest() {
      foreach (var dir in new DirectoryInfo(PlanesPath).GetDirectories()) {
        var name = dir.Name;
        var file = new FileInfo(Path.Combine(dir.FullName, $"{name}.txt"));
        
        if (!file.Exists) {
          continue;
        }

        var aircraft = Aircraft.Load(dir);
      }
    }

    [Fact]
    public void SerializationTest() {
      var aircraft = Aircraft.Load(ASK21Path);
      var json = JsonConvert.SerializeObject(aircraft, Formatting.Indented);
      var aircraft2 = JsonConvert.DeserializeObject<Aircraft>(json);

      Assert.Equal("Alexander Schleicher ASK21", aircraft2.Name);
      Assert.Equal("Alexander Schleicher ASK21", aircraft.ToString());
      Assert.Equal(17, aircraft2.WingSpan);
      Assert.Equal(17.95, aircraft2.WingArea);
      Assert.Equal(8.35, aircraft2.Length);
      Assert.Equal(360, aircraft2.MassEmpty);
      Assert.Equal(440, aircraft2.MassReference);
      Assert.Equal(600, aircraft2.MassMax);
      Assert.Equal(0, aircraft2.WaterBallast);
      Assert.Equal(24.0, aircraft2.WingLoadingMin, 1);
      Assert.Equal(33.4, aircraft2.WingLoadingMax, 1);
      Assert.Equal(280, aircraft2.SpeedMax);
      Assert.Equal(180, aircraft2.SpeedManeuvering);
      Assert.Equal(63, aircraft2.SpeedMin, 0);
      Assert.Equal(-0.65, aircraft2.SinkMin, 2);
      Assert.Equal(33.4, aircraft2.GlideRatioBest, 1);
      Assert.Equal(87, aircraft2.SpeedBestGlide, 0);
      Assert.Equal(92, aircraft2.DAeCIndex);
      Assert.Equal(5, aircraft2.Polar.Count);
    }
  }
}
