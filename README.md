# Aiursoft.BadgeProxy

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://gitlab.aiursoft.com/aiursoft/badgeproxy/-/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.com/aiursoft/badgeproxy/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/badgeproxy/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.com/aiursoft/badgeproxy/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/badgeproxy/-/pipelines)
[![NuGet version (Aiursoft.BadgeProxy)](https://img.shields.io/nuget/v/Aiursoft.BadgeProxy.svg)](https://www.nuget.org/packages/Aiursoft.BadgeProxy/)
[![Man hours](https://manhours.aiursoft.com/r/gitlab.aiursoft.com/aiursoft/BadgeProxy.svg)](https://manhours.aiursoft.com/r/gitlab.aiursoft.com/aiursoft/BadgeProxy.html)
[![Docker](https://img.shields.io/docker/pulls/aiursoft/badgeproxy.svg)](https://hub.docker.com/r/aiursoft/badgeproxy)

BadgeProxy is a proxy service and CLI tool for GitLab pipeline status and test coverage badge SVGs.

## Features

- **High-Performance Caching**: In-memory caching with configurable TTL to reduce load on the GitLab API.
- **Private Repository Support**: Transparently resolves branch pipeline and test coverage metrics even for private repositories where `default_branch` is restricted to Guest tokens.
- **Concurrency Limiting**: Built-in semaphore to throttle concurrent upstream requests and protect against rate-limiting.
- **Health Check Probe**: Built-in `--healthcheck` client option for easy integration with Docker health checks and container orchestrators.

## Install

Requirements:

1. [.NET 10 SDK](http://dot.net/)

Run the following command to install this tool:

```bash
dotnet tool install --global Aiursoft.BadgeProxy
```

## Usage

After getting the binary, run it directly in the terminal:

```bash
$ badgeproxy --help
Description:
  BadgeProxy is a proxy service for GitLab pipeline status and coverage badge SVGs.

Usage:
  badgeproxy [options]

Options:
  -p, --port <port>              The port to listen for the server. [default: 5000]
  --gitlab-url <gitlab-url>      The GitLab instance URL. [default: https://gitlab.aiursoft.com]
  --gitlab-token <gitlab-token>  The GitLab Personal Access Token for querying badges.
  --healthcheck                  Run health check against the local running service.
  --version                      Show version information
  -?, -h, --help                 Show help and usage information
```

### Environment Variables

You can also pass the GitLab access token via environment variable:

```bash
export GITLAB_TOKEN="glpat-xxxxxxxxxxxxxxxxxxxx"
badgeproxy --port 5000
```

### Endpoints

BadgeProxy serves badge SVGs matching the GitLab badge URL structure:

- Pipeline status badge:
  `GET /{namespace}/{project}/badges/{branch}/pipeline.svg`
- Test coverage badge:
  `GET /{namespace}/{project}/badges/{branch}/coverage.svg`

## Run in Docker

First, install Docker [here](https://docs.docker.com/get-docker/).

Then run the following commands in a Linux shell:

```bash
image=aiursoft/badgeproxy
appName=badgeproxy

sudo docker pull $image
sudo docker run -d \
  --name $appName \
  --restart unless-stopped \
  -p 5000:5000 \
  -e GITLAB_TOKEN="glpat-xxxxxxxxxxxxxxxxxxxx" \
  $image
```

The docker image has the following context:

| Properties  | Value                 |
|-------------|-----------------------|
| Image       | aiursoft/badgeproxy   |
| Ports       | 5000                  |
| Binary path | /app/badgeproxy       |

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
