#!/bin/sh
set -e
 
CONTAINER_NAME='authentication-service'
IMAGE_FAMILY='authentication-service'
 
docker build -t "$IMAGE_FAMILY:latest" .
CONTAINER_ID=$(docker ps -a -q --filter "name=$CONTAINER_NAME" || true)
if [ -n "$CONTAINER_ID" ]; then
    docker stop $CONTAINER_NAME || true
    docker rm $CONTAINER_NAME || true
fi
 
CONTAINER_ID=$(docker create --name "$CONTAINER_NAME" --publish 44331:44331 --publish 44332:44332 "$IMAGE_FAMILY")
docker cp "$(pwd)/config/local/local-api.xp3riment.net/local-api.xp3riment.net.pfx" "$CONTAINER_ID:/app/"
docker cp "$(pwd)/config/local/signing-credential/signing-credential.pfx" "$CONTAINER_ID:/app/"
docker start "$CONTAINER_NAME"
echo "Container started: $CONTAINER_ID"
exit 0