FROM mcr.microsoft.com/dotnet/sdk:10.0.101 AS build

WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0.1

WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "LowSharp.Cli.dll", "grpc", "serve"]