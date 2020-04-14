using Microsoft.Extensions.Configuration;
using System.IO;
using Tests.Model;

namespace Tests.Extensions {
  internal static class ConfigurationExtensions {
    private const string CondorPathKey = "CondorPath";

    private static IConfiguration Configuration => new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", false)
          .AddJsonFile("appsettings.user.json", false)
          .Build();

    internal static IConfiguration GetConfiguration(this ICondorTest test) => Configuration;

    internal static string GetPlanesPath(this ICondorTest test) {
      return Path.Combine(Configuration[CondorPathKey], "Planes");
    }

    internal static string GetPolarFile(this ICondorTest test, string name) {
      return Path.Combine(test.GetPlanesPath(), name, $"{name}.pol");
    }
  }
}
