using Aiursoft.BadgeProxy.Handlers;

namespace Aiursoft.BadgeProxy.Tests;

[TestClass]
public class HealthCheckTests
{
    [TestMethod]
    public async Task TestCheckHealthWhenNotRunning()
    {
        // Port 59999 should not have a badgeproxy server running
        var healthy = await BadgeProxyHandler.CheckHealthAsync(59999);
        Assert.IsFalse(healthy);
    }
}
