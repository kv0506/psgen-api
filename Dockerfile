FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/PsGenApi.csproj", "src/"]
RUN dotnet restore "src/PsGenApi.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "PsGenApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PsGenApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PsGenApi.dll"]
