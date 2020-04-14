using CondorDotNet.Extensions;
using CondorDotNet.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests {
  public class ConversionTests {
    [Fact]
    public void MassTest() {
      Assert.Equal(264.550, 120.0.ConvertMass(UnitType.Imperial), 3);
      Assert.Equal(264.550, 120.0.ConvertMass(UnitType.Imperial, UnitType.Australian), 3);

      Assert.Equal(136.08, 300.0.ConvertMass(UnitType.Metric, UnitType.Imperial), 3);
      Assert.Equal(136.08, 300.0.ConvertMass(UnitType.Australian, UnitType.Imperial), 3);

      // None needed
      Assert.Equal(5, 5.0.ConvertMass(UnitType.Metric));
      Assert.Equal(5, 5.0.ConvertMass(UnitType.Australian));
    }

    [Fact]
    public void SinkRateTest() {
      Assert.Equal(9.719, 5.0.ConvertSinkRate(UnitType.Imperial), 3);
      Assert.Equal(9.719, 5.0.ConvertSinkRate(UnitType.Imperial, UnitType.Australian), 3);

      Assert.Equal(4.630, 9.0.ConvertSinkRate(UnitType.Metric, UnitType.Imperial), 3);
      Assert.Equal(4.630, 9.0.ConvertSinkRate(UnitType.Australian, UnitType.Imperial), 3);

      // None needed
      Assert.Equal(5, 5.0.ConvertSinkRate(UnitType.Metric));
      Assert.Equal(5, 5.0.ConvertSinkRate(UnitType.Australian));
    }

    [Fact]
    public void SpeedTest() {
      Assert.Equal(59.395, 110.0.ConvertSpeed(UnitType.Imperial), 3);
      Assert.Equal(59.395, 110.0.ConvertSpeed(UnitType.Australian), 3);

      Assert.Equal(111.12, 60.0.ConvertSpeed(UnitType.Metric, UnitType.Imperial), 3);
      Assert.Equal(111.12, 60.0.ConvertSpeed(UnitType.Metric, UnitType.Australian), 3);

      // None needed
      Assert.Equal(100, 100.0.ConvertSpeed(UnitType.Metric));
      Assert.Equal(100, 100.0.ConvertSpeed(UnitType.Imperial, UnitType.Australian));
    }

    [Fact]
    public void VolumeTest() {
      Assert.Equal(158.503, 600.0.ConvertVolume(UnitType.Imperial), 3);
      Assert.Equal(158.503, 600.0.ConvertVolume(UnitType.Imperial, UnitType.Australian), 3);

      Assert.Equal(757.082, 200.0.ConvertVolume(UnitType.Metric, UnitType.Imperial), 3);
      Assert.Equal(757.082, 200.0.ConvertVolume(UnitType.Australian, UnitType.Imperial), 3);

      // None needed
      Assert.Equal(5, 5.0.ConvertVolume(UnitType.Metric));
      Assert.Equal(5, 5.0.ConvertVolume(UnitType.Australian));
    }

    [Fact]
    public void WingLoadingTest() {
      Assert.Equal(6.841, 33.4.ConvertWingLoading(UnitType.Imperial), 3);
      Assert.Equal(6.841, 33.4.ConvertWingLoading(UnitType.Imperial, UnitType.Australian), 3);

      Assert.Equal(43.943, 9.0.ConvertWingLoading(UnitType.Metric, UnitType.Imperial), 3);
      Assert.Equal(43.943, 9.0.ConvertWingLoading(UnitType.Australian, UnitType.Imperial), 3);

      // None needed
      Assert.Equal(5, 5.0.ConvertWingLoading(UnitType.Metric));
      Assert.Equal(5, 5.0.ConvertWingLoading(UnitType.Australian));
    }
  }
}
