using Aiursoft.BadgeProxy.Handlers;
using Aiursoft.CommandFramework;

namespace Aiursoft.BadgeProxy.Tests;

[TestClass]
public class CliTests
{
    private readonly SingleCommandApp<BadgeProxyHandler> _program = new();

    [TestMethod]
    public async Task InvokeHelp()
    {
        var result = await _program.TestRunAsync(["--help"]);
        Assert.AreEqual(0, result.ProgramReturn);
    }

    [TestMethod]
    public async Task InvokeVersion()
    {
        var result = await _program.TestRunAsync(["--version"]);
        Assert.AreEqual(0, result.ProgramReturn);
    }

    [TestMethod]
    public async Task InvokeUnknown()
    {
        var result = await _program.TestRunAsync(["--unknown-arg"]);
        Assert.AreEqual(1, result.ProgramReturn);
    }
}
