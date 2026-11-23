# ============================
# Prepare Building Environment
FROM --platform=$BUILDPLATFORM hub.aiursoft.com/aiursoft/internalimages/dotnet AS build-env
ARG TARGETARCH
ARG NUGET_SOURCE=https://api.nuget.org/v3/index.json
WORKDIR /src
COPY . .
RUN if [ "$TARGETARCH" = "arm64" ]; then \
        RID="linux-arm64"; \
    elif [ "$TARGETARCH" = "amd64" ]; then \
        RID="linux-x64"; \
    else \
        RID="linux-$TARGETARCH"; \
    fi && \
    echo "Building for arch: $TARGETARCH, using .NET RID: $RID" && \
    dotnet publish ./src/Aiursoft.BadgeProxy/Aiursoft.BadgeProxy.csproj --configuration Release --no-self-contained --runtime $RID --output /app -p:UseAppHost=false --configfile nuget.config -p:RestoreSources="$NUGET_SOURCE"

# ============================
# Prepare Runtime Environment
FROM hub.aiursoft.com/aiursoft/internalimages/dotnetonlyruntime
WORKDIR /app
COPY --from=build-env /app .

EXPOSE 5000
HEALTHCHECK --interval=60s --timeout=5s --start-period=10s --retries=3 CMD ["dotnet", "/app/badgeproxy.dll", "--healthcheck", "--port", "5000"]
ENTRYPOINT ["dotnet", "/app/badgeproxy.dll", "--port", "5000"]
