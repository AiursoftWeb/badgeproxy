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
    public void TestRenderCoverageGood()
    {
        var svg = SvgBadge.Render("coverage", "97.92%");
        Assert.IsTrue(svg.Contains("coverage"));
        Assert.IsTrue(svg.Contains("97.92%"));
        Assert.IsTrue(svg.Contains("#4c1"));
    }

    [TestMethod]
    public void TestRenderCoverageAcceptable()
    {
        var svg = SvgBadge.Render("coverage", "92.5%");
        Assert.IsTrue(svg.Contains("coverage"));
        Assert.IsTrue(svg.Contains("92.5%"));
        Assert.IsTrue(svg.Contains("#a3c51c"));
    }

    [TestMethod]
    public void TestRenderCoverageMedium()
    {
        var svg = SvgBadge.Render("coverage", "85.0%");
        Assert.IsTrue(svg.Contains("coverage"));
        Assert.IsTrue(svg.Contains("85.0%"));
        Assert.IsTrue(svg.Contains("#dfb317"));
    }

    [TestMethod]
    public void TestRenderCoverageLow()
    {
        var svg = SvgBadge.Render("coverage", "23.31%");
        Assert.IsTrue(svg.Contains("coverage"));
        Assert.IsTrue(svg.Contains("23.31%"));
        Assert.IsTrue(svg.Contains("#e05d44"));
    }

    [TestMethod]
    public void TestRenderCoverageUnknown()
    {
        var svg = SvgBadge.Render("coverage", "unknown");
        Assert.IsTrue(svg.Contains("coverage"));
        Assert.IsTrue(svg.Contains("unknown"));
        Assert.IsTrue(svg.Contains("#9f9f9f"));
    }

    [TestMethod]
    public void TestRenderPipelineUnknown()
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
