# Aiursoft BadgeProxy

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://gitlab.aiursoft.com/aiursoft/badgeproxy/-/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.com/aiursoft/badgeproxy/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/badgeproxy/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.com/aiursoft/badgeproxy/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/badgeproxy/-/pipelines)
[![NuGet version (Aiursoft.BadgeProxy)](https://img.shields.io/nuget/v/Aiursoft.BadgeProxy.svg)](https://www.nuget.org/packages/Aiursoft.BadgeProxy/)
[![Man hours](https://manhours.aiursoft.com/r/gitlab.aiursoft.com/aiursoft/BadgeProxy.svg)](https://manhours.aiursoft.com/r/gitlab.aiursoft.com/aiursoft/BadgeProxy.html)
[![Docker](https://img.shields.io/docker/pulls/aiursoft/badgeproxy.svg)](https://hub.docker.com/r/aiursoft/badgeproxy)

BadgeProxy is a proxy service and CLI tool for GitLab pipeline status and test coverage badge SVGs.

## Overview

### What is BadgeProxy?

BadgeProxy serves GitLab pipeline status and test coverage as SVG badges for projects in the `aiursoft` namespace. It supports private projects that the configured GitLab token can read. When GitLab does not expose the default branch to a Guest token, BadgeProxy uses the branch in the badge URL.

An in-memory cache and concurrent request limits reduce load on GitLab. The built-in health probe can be used by container orchestrators.

## Installation

Install the CLI with the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0):

```bash
dotnet tool install --global Aiursoft.BadgeProxy
```

Update an existing installation:

```bash
dotnet tool update --global Aiursoft.BadgeProxy
```

Remove the CLI:

```bash
dotnet tool uninstall --global Aiursoft.BadgeProxy
```

**Docker**

Install [Docker](https://docs.docker.com/get-docker/) and set `GITLAB_ACCESS_TOKEN` in your shell before starting the container:

```bash
image=aiursoft/badgeproxy
appName=badgeproxy

sudo docker pull "$image"
sudo docker run -d \
  --name "$appName" \
  --restart unless-stopped \
  -p 5000:5000 \
  -e GITLAB_URL=https://gitlab.aiursoft.com \
  -e GITLAB_ACCESS_TOKEN="$GITLAB_ACCESS_TOKEN" \
  "$image"
```

The image listens on port `5000` and starts with `dotnet /app/badgeproxy.dll`.

**From source**

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and follow the build and run instructions in [Development](#development).

## Usage

Run the command to see available options:

```bash
badgeproxy --help
```

With a GitLab access token configured, start the service:

```bash
badgeproxy --gitlab-url https://gitlab.aiursoft.com --port 5000
```

To probe the running service locally, run `badgeproxy --healthcheck --port 5000`.

### Endpoints

- `GET /aiursoft/{project}/badges/{branch}/pipeline.svg` returns the pipeline status badge.
- `GET /aiursoft/{project}/badges/{branch}/coverage.svg` returns the test coverage badge.
- `GET /health` responds to local health probes.

Use the default branch when GitLab exposes it to the configured token.

## Configuration

Set `GITLAB_ACCESS_TOKEN` to a GitLab access token with `read_api` scope and access to the projects whose badges you want to serve. Set `GITLAB_URL` to the GitLab origin when running outside the internal Docker network. Command-line values take precedence over environment variables.

```bash
export GITLAB_ACCESS_TOKEN="<your GitLab access token>"
export GITLAB_URL=https://gitlab.aiursoft.com
```

**Command-line options**

| Option | Description |
| --- | --- |
| `-p, --port <port>` | HTTP port. Defaults to `5000`. |
| `-u, --gitlab-url <url>` | GitLab origin. Defaults to `GITLAB_URL` or `http://gitlab_gitlab`. |
| `-t, --gitlab-token <token>` | GitLab access token. Defaults to `GITLAB_ACCESS_TOKEN`. Required when serving requests. |
| `--healthcheck` | Probe the local service and exit. |
| `--version` | Show version information. |
| `-?, -h, --help` | Show help and usage information. |

## Development

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

From the repository root, restore and build:

```bash
dotnet restore Aiursoft.BadgeProxy.sln
dotnet build Aiursoft.BadgeProxy.sln --no-restore
```

Run the tool from source:

```bash
dotnet run --project src/Aiursoft.BadgeProxy -- --help
```

With the environment variables in [Configuration](#configuration) set, replace `--help` with `--port 5000` to start the service.

Run the existing tests:

```bash
dotnet test Aiursoft.BadgeProxy.sln --no-restore
```

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.

## License

Licensed under the [MIT License](LICENSE).
