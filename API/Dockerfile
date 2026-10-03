FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TechStore Cloud/TechStore Cloud.csproj", "TechStore Cloud/"]
COPY ["TechStoreCloud.Infra/TechStoreCloud.Infra.csproj", "TechStoreCloud.Infra/"]
COPY ["TechStoreCloud.DomainModel/TechStoreCloud.DomainModel.csproj", "TechStoreCloud.DomainModel/"]
COPY ["TechStoreCloud.DomainService/TechStoreCloud.DomainService.csproj", "TechStoreCloud.DomainService/"]
RUN dotnet restore "TechStore Cloud/TechStore Cloud.csproj"
COPY . .
RUN dotnet publish "TechStore Cloud/TechStore Cloud.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TechStore Cloud.dll"]
