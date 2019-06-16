# Builder image
FROM microsoft/dotnet:2.2-sdk AS build

WORKDIR /src

# copy csproj and restore as distinct layers
COPY src/ ./
RUN dotnet restore AuthenticationService.sln
RUN dotnet build AuthenticationService.sln --no-restore --configuration Release
RUN dotnet publish AuthenticationService/AuthenticationService.csproj --no-restore --no-build --configuration Release --output /out
RUN ls /out

# Runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]