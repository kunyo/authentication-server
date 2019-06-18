#!/bin/sh
set -e
 
CONTAINER_NAME='authentication-service'
IMAGE_FAMILY='authentication-service'
 
docker build -t "$IMAGE_FAMILY" .
CONTAINER_ID=$(docker ps -a -q --filter "name=$CONTAINER_NAME" || true)
if [ -n "$CONTAINER_ID" ]; then
    docker stop $CONTAINER_NAME || true
    docker rm $CONTAINER_NAME || true
fi
 
CONTAINER_ID=$(docker create --name "$CONTAINER_NAME" --publish 8080:80 "$IMAGE_FAMILY")
docker start "$CONTAINER_NAME"
exit 0