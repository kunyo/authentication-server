#!/bin/sh
set -e
TARGET_ENV=$1
CONTAINER_NAME="authentication-service"
IMAGE_NAME="eimpresa/authentication-service"
KEY_DIR=$(realpath "$(dirname $0)/scripts/keys")

test -n "$TARGET_ENV" || (echo "Usage: $0 <target environment>"; exit 1)
test -d "$KEY_DIR" || (echo "Key dir does not exist: $KEY_DIR"; exit 1)
test -f "$KEY_DIR/$TARGET_ENV/cert.pfx" || (echo "Environment not found: $TARGET_ENV"; exit 1)

(docker stop "$CONTAINER_NAME" && docker rm "$CONTAINER_NAME") || true
CONTAINER_ID=$(docker create --name "$CONTAINER_NAME" "$IMAGE_NAME")
docker cp "$KEY_DIR/$TARGET_ENV/cert.pfx" "$CONTAINER_ID:/app/cert.pfx"
docker start $CONTAINER_ID
echo "Deploy OK"
exit 0