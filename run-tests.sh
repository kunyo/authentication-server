#!/bin/sh
set -x
CONTAINER_NAME="authentication-service"
TEST_ENVIRONMENT="$1"
TEST_PROJECT=$(realpath ./src/AuthenticationService.SmokeTests/AuthenticationService.SmokeTests.csproj)

if [ -z "$TEST_ENVIRONMENT" ]; then
    TEST_ENVIRONMENT="local"
fi

TEST_ENVIRONMENT=$TEST_ENVIRONMENT dotnet test $TEST_PROJECT
exit 0