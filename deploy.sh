#!/bin/sh
set -e
TARGET_ENV="dev"
CONTAINER_NAME="authentication-service"
IMAGE_NAME="eimpresa/authentication-service"
KEY_DIR="$(dirname $0)/config/$TARGET_ENV"

test -n "$TARGET_ENV" || (echo "Usage: $0 <target environment>"; exit 1)
test -d "$KEY_DIR" || (echo "Key dir does not exist: $KEY_DIR"; exit 1)

docker stop "$CONTAINER_NAME" || true
docker rm "$CONTAINER_NAME" || true
CONTAINER_ID=$(docker create --name "$CONTAINER_NAME" --publish 44332:44331 "$IMAGE_NAME")
docker cp "$KEY_DIR/dev-api.xp3riment.net/dev-api.xp3riment.net.pfx" "$CONTAINER_ID:/app/dev-api.xp3riment.net.pfx"  > /dev/null
docker cp "$KEY_DIR/signing-credential/signing-credential.pfx" "$CONTAINER_ID:/app/signing-credential.pfx"  > /dev/null
docker start $CONTAINER_ID  > /dev/null
echo "protocol = https"
echo "hostname = localhost"
echo "port = 44332"
exit 0