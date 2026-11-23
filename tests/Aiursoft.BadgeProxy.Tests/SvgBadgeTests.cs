using Aiursoft.BadgeProxy.Services;

namespace Aiursoft.BadgeProxy.Tests;

[TestClass]
public class SvgBadgeTests
{
    [TestMethod]
    public void TestRenderPipelinePassed()
    {
        var svg = SvgBadge.Render("pipeline", "passed");
        Assert.IsTrue(svg.Contains("pipeline"));
        Assert.IsTrue(svg.Contains("passed"));
        Assert.IsTrue(svg.Contains("#4c1"));
    }

    [TestMethod]
    public void TestRenderPipelineFailed()
    {
        var svg = SvgBadge.Render("pipeline", "failed");
        Assert.IsTrue(svg.Contains("pipeline"));
        Assert.IsTrue(svg.Contains("failed"));
        Assert.IsTrue(svg.Contains("#e05d44"));
    }

    [TestMethod]
    public void TestRenderCoverage()
    {
        var svg = SvgBadge.Render("coverage", "23.31%");
        Assert.IsTrue(svg.Contains("coverage"));
        Assert.IsTrue(svg.Contains("23.31%"));
        Assert.IsTrue(svg.Contains("#007ec6"));
    }

    [TestMethod]
    public void TestRenderUnknown()
    {
        var svg = SvgBadge.Render("pipeline", "unknown");
        Assert.IsTrue(svg.Contains("pipeline"));
        Assert.IsTrue(svg.Contains("unknown"));
        Assert.IsTrue(svg.Contains("#9f9f9f"));
    }

    [TestMethod]
    public void TestSecurityEscaping()
    {
        var svg = SvgBadge.Render("<script>", "val&1");
        Assert.IsFalse(svg.Contains("<script>"));
        Assert.IsTrue(svg.Contains("&lt;script&gt;"));
        Assert.IsTrue(svg.Contains("val&amp;1"));
    }
}
