# Builder image
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /build
COPY /src/ .
RUN dotnet restore AuthenticationService.sln
RUN dotnet build AuthenticationService.sln --no-restore --configuration Release
RUN dotnet publish AuthenticationService/AuthenticationService.csproj --no-restore --no-build --configuration Release --output /out/
RUN ls

# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime
WORKDIR /app
COPY --from=build /out/ .
RUN ls
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]