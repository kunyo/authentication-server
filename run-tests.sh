#!/bin/sh
set -e

TEST_FILTER="$1"
if [ -z "$TEST_FILTER" ]; then TEST_FILTER="*.UnitTests.csproj"; fi

SOURCE_DIR=$(realpath ./src/)

ran_count=0
for f in $(find "$SOURCE_DIR" -type f -name "$TEST_FILTER"); do
    dotnet test $f || true;
    ran_count=$((ran_count + 1))
done

if [ $ran_count -eq 0 ]; then
    echo "No test project found in source dir: $SOURCE_DIR"
    exit 1
fi

exit 0