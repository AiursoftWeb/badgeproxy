using System.CommandLine;
using System.Globalization;
using Aiursoft.BadgeProxy.Services;
using Aiursoft.CommandFramework.Framework;

namespace Aiursoft.BadgeProxy.Handlers;

public class BadgeProxyHandler : ExecutableCommandHandlerBuilder
{
    private static readonly HttpClient HealthProbeClient = new() { Timeout = TimeSpan.FromSeconds(3) };

    protected override string Name => "badgeproxy";

    protected override string Description => "Start GitLab badge proxy service.";

    protected override Option[] GetCommandOptions() =>
    [
        OptionsProvider.PortOption,
        OptionsProvider.GitLabUrlOption,
        OptionsProvider.GitLabTokenOption,
        OptionsProvider.HealthCheckOption
    ];

    protected override async Task Execute(ParseResult context)
    {
        var port = context.GetValue(OptionsProvider.PortOption);
        var isHealthCheck = context.GetValue(OptionsProvider.HealthCheckOption);
        if (isHealthCheck)
        {
            var healthy = await CheckHealthAsync(port);
            Environment.ExitCode = healthy ? 0 : 1;
            return;
        }

        var gitlabUrl = context.GetValue(OptionsProvider.GitLabUrlOption);
        if (string.IsNullOrWhiteSpace(gitlabUrl))
        {
            gitlabUrl = Environment.GetEnvironmentVariable("GITLAB_URL");
        }
        if (string.IsNullOrWhiteSpace(gitlabUrl))
        {
            gitlabUrl = "http://gitlab_gitlab";
        }

        var token = context.GetValue(OptionsProvider.GitLabTokenOption);
        if (string.IsNullOrWhiteSpace(token))
        {
            token = Environment.GetEnvironmentVariable("GITLAB_ACCESS_TOKEN");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("GITLAB_ACCESS_TOKEN is required. Pass --gitlab-token or set GITLAB_ACCESS_TOKEN environment variable.");
        }

        var app = BuildApp(gitlabUrl, token, port);
        await app.RunAsync();
    }

    internal static async Task<bool> CheckHealthAsync(int port)
    {
        try
        {
            using var response = await HealthProbeClient.GetAsync($"http://127.0.0.1:{port}/health");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException) { return false; }
        catch (TaskCanceledException) { return false; }
    }

    internal static WebApplication BuildApp(string gitlabUrl, string token, int port)
    {
        if (!Uri.TryCreate(gitlabUrl.TrimEnd('/') + "/api/v4/", UriKind.Absolute, out var api) ||
            api.Scheme is not ("http" or "https") || !string.IsNullOrEmpty(api.UserInfo) ||
            !string.IsNullOrEmpty(api.Query) || !string.IsNullOrEmpty(api.Fragment) || api.AbsolutePath != "/api/v4/")
            throw new InvalidOperationException("GITLAB_URL must be an HTTP(S) origin.");

        var builder = WebApplication.CreateBuilder();
        builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        var client = new HttpClient(new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            MaxConnectionsPerServer = 4
        })
        {
            BaseAddress = api,
            Timeout = Timeout.InfiniteTimeSpan
        };
        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", token);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Aiursoft-BadgeProxy/1.0");

        builder.Services.AddSingleton(client);
        builder.Services.AddSingleton<GitLabBadges>();

        var app = builder.Build();
        var badgeService = app.Services.GetRequiredService<GitLabBadges>();

        app.MapGet("/health", () => Results.Text("healthy"));
        app.MapMethods("/aiursoft/{project}/badges/{**badgePath}", ["GET", "HEAD"], async (HttpContext context, string project, string? badgePath) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            if (project.Length > 128 || !System.Text.RegularExpressions.Regex.IsMatch(project, "^[a-zA-Z0-9][a-zA-Z0-9_.-]*$", System.Text.RegularExpressions.RegexOptions.CultureInvariant))
                return Results.NotFound();
            var slash = badgePath?.LastIndexOf('/') ?? -1;
            if (slash < 1) return Results.NotFound();
            var branch = badgePath![..slash];
            var file = badgePath[(slash + 1)..];
            if (file is not ("pipeline.svg" or "coverage.svg")) return Results.NotFound();
            if (context.Request.QueryString.HasValue)
                return Results.BadRequest();
            var snapshot = await badgeService.GetAsync(project.ToLowerInvariant(), branch, context.RequestAborted);
            if (!snapshot.Found || snapshot.DefaultBranch is not null && branch != snapshot.DefaultBranch)
                return Results.NotFound();
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers.ContentSecurityPolicy = "default-src 'none'; sandbox";
            var coverage = file == "coverage.svg";
            var value = coverage ? snapshot.Coverage?.ToString("0.##", CultureInfo.InvariantCulture) + "%" : snapshot.Status;
            if (coverage && snapshot.Coverage is null) value = "unknown";
            return Results.Text(SvgBadge.Render(coverage ? "coverage" : "pipeline", value), "image/svg+xml; charset=utf-8");
        });

        return app;
    }
}
