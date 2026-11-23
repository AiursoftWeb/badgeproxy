using System.CommandLine;

namespace Aiursoft.BadgeProxy;

public static class OptionsProvider
{
    public static readonly Option<int> PortOption = new(
        name: "--port",
        aliases: ["-p"])
    {
        DefaultValueFactory = _ => 5000,
        Description = "The port to listen on.",
        Required = false,
    };

    public static readonly Option<string?> GitLabUrlOption = new(
        name: "--gitlab-url",
        aliases: ["-u"])
    {
        Description = "The GitLab base URL (e.g. https://gitlab.aiursoft.com). Defaults to GITLAB_URL environment variable or http://gitlab_gitlab.",
        Required = false,
    };

    public static readonly Option<string?> GitLabTokenOption = new(
        name: "--gitlab-token",
        aliases: ["-t"])
    {
        Description = "The GitLab access token with read API scope. Defaults to GITLAB_ACCESS_TOKEN environment variable.",
        Required = false,
    };

    public static readonly Option<bool> HealthCheckOption = new(
        name: "--healthcheck")
    {
        DefaultValueFactory = _ => false,
        Description = "Run healthcheck probe against the service and exit with 0 if healthy.",
        Required = false,
    };
}
