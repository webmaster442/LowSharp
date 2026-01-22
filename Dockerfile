# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ./Lowsharp.Server ./Server
COPY ./Protobuff ./Protobuff
WORKDIR /src/Server
RUN dotnet restore
RUN dotnet build --no-restore -c Release
RUN dotnet publish --no-build -c Release -o /app/publish

# Runtime stage
FROM debian:stable-slim AS runtime

RUN apt-get update && apt-get install -y wget
RUN wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install -y dotnet-sdk-10.0
RUN rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
 # gRPC port
EXPOSE 11483
 # HTTP port
EXPOSE 11484

ENTRYPOINT ["dotnet", "Lowsharp.Server.dll"]

# Build command with date tag:
# docker build -t lowsharp.server:$(date +%Y.%m.%d) .
