using CondorDotNet.Model;
using System.IO;
using Tests.Extensions;
using Tests.Model;
using Xunit;

namespace Tests {
  public class GdiTests : ICondorTest {
    private string OutputPath => this.GetConfiguration()["OutputPath"];

    [Fact]
    public void BitmapTest() {
      var factory = new BitmapFactory() {
        Aircraft = Aircraft.Load(Path.Combine(this.GetPlanesPath(), "ASK21")),
        //Units = UnitType.Imperial
      };

      factory.Mass = new double[] { factory.Aircraft.MassReference, factory.Aircraft.MassReference + 80, factory.Aircraft.MassMax };

      using (var img = factory.Generate()) {
        img.Save(Path.Combine(OutputPath, $"{factory.Aircraft.Name}.png"), System.Drawing.Imaging.ImageFormat.Png);
      }
    }
  }
}
