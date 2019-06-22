#!/bin/sh
set -e

TARGET_ENV="$1"
ECR_REPOSITORY_URI="353153972523.dkr.ecr.eu-west-1.amazonaws.com/authentication-server-dev"
ECS_CLUSTER_NAME="authentication-server-$TARGET_ENV"
ECS_SERVICE_NAME="authentication-server-$TARGET_ENV"
DOCKER_IMAGE_NAME="authentication-service:latest"
APP_CONFIGURATION_FILE="$(pwd)/config/$TARGET_ENV/appsettings.json"

test -n "$TARGET_ENV" || (>&2 echo "Usage: $0 <environment name>"; exit 1)
test -n "$AWS_PROFILE" || (>&2 echo "AWS_PROFILE environment variable must be defined."; exit 1)
test -n "$AWS_REGION" || (>&2 echo "AWS_REGION environment variable must be defined."; exit 1)

# Get the docker image id
DOCKER_IMAGE_ID="$(docker images -q $DOCKER_IMAGE_NAME)"
test -n "$DOCKER_IMAGE_ID" || (>&2 echo "Docker image not found on local repository: $DOCKER_IMAGE_NAME")
test -f "$APP_CONFIGURATION_FILE" || (>&2 echo "App configuration file not found: $APP_CONFIGURATION_FILE")

echo "aws_profile = $AWS_PROFILE"
echo "aws_region = $AWS_REGION"
echo "ec_repository_uri = $ECR_REPOSITORY_URI"
echo "ecs_cluster_name = $ECS_CLUSTER_NAME"
echo "ecs_service_name = $ECS_SERVICE_NAME"
echo "docker_image_name = $DOCKER_IMAGE_NAME"
echo "docker_image_id = $DOCKER_IMAGE_ID"
echo "app_configuration_file = $APP_CONFIGURATION_FILE"
echo
read -p "Is this OK? (Yes): " force
[ "$force" = "Yes" ] || (echo "Nothing done, exiting..."; exit 1)

# Creates the deployment image
TMP_CONTAINER_ID=$(docker create $DOCKER_IMAGE_ID)

# Copy the application configuration to the app directory specified in the DockerFile
docker cp "$APP_CONFIGURATION_FILE" "$TMP_CONTAINER_ID:/app/appsettings.json"

# Commit the temporary container and tag it with the target repository uri
docker commit $TMP_CONTAINER_ID "$ECR_REPOSITORY_URI:latest"

# Get the login command for the ECR Repository
echo "Getting ECR repository credential"
DOCKER_LOGIN_CMD="$(aws ecr get-login --profile $AWS_PROFILE --region $AWS_REGION --no-include-email)"

# Push the image to the repository
echo "Pushing the image to ECR"
$DOCKER_LOGIN_CMD
docker push "$ECR_REPOSITORY_URI"

# Remove the temporary image and container
docker rm "$TMP_CONTAINER_ID"
docker rmi "$ECR_REPOSITORY_URI:latest"

# Run task
echo "Forcing service redeployment"
aws ecs update-service --profile "$AWS_PROFILE" --region "$AWS_REGION" --cluster "$ECS_CLUSTER_NAME" --service "$ECS_SERVICE_NAME" --force-new-deployment

echo "Deployment triggered"
exit 0