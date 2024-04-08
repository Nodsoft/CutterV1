FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Nodsoft.Cutter.Web/Nodsoft.Cutter.Web.csproj", "Nodsoft.Cutter.Web/"]
RUN dotnet restore "Nodsoft.Cutter.Web/Nodsoft.Cutter.Web.csproj"
COPY . .
WORKDIR "/src/Nodsoft.Cutter.Web"
RUN dotnet build "Nodsoft.Cutter.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nodsoft.Cutter.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nodsoft.Cutter.Web.dll"]
