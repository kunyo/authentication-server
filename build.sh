#/!bin/sh
set -ex

PROJECT_DIR=$(realpath $(dirname $0))
BUILD_DIR=$PROJECT_DIR/build
DIST_DIR=$PROJECT_DIR/dist
SOLUTION_FILE=$PROJECT_DIR/src/AuthenticationService.sln
WEBAPP_PROJECT_FILE=$PROJECT_DIR/src/AuthenticationService/AuthenticationService.csproj
BUILD_CONFIGURATION=Release
ARTIFACT_NAME=authentication-service.tar.gz

test -d "$BUILD_DIR" && rm -rf "$BUILD_DIR"
test -d "$DIST_DIR" && rm -rf "$DIST_DIR"

dotnet --version
dotnet clean "$SOLUTION_FILE" --configuration "$BUILD_CONFIGURATION"
dotnet restore "$SOLUTION_FILE" --verbosity Detailed
dotnet build "$SOLUTION_FILE" --no-restore --configuration "$BUILD_CONFIGURATION"
dotnet publish "$WEBAPP_PROJECT_FILE" --no-restore --no-build --configuration "$BUILD_CONFIGURATION" --output "$BUILD_DIR"

mkdir "$DIST_DIR"
cd "$BUILD_DIR"
tar cvfz "$DIST_DIR/$ARTIFACT_NAME" *

echo "Build OK"
exit 0
