namespace Aiursoft.BadgeProxy.Services;

public sealed record Snapshot(
    string Status,
    double? Coverage,
    DateTimeOffset Expires,
    string? DefaultBranch = null,
    bool Found = true);
