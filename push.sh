#!/bin/sh
set -e

AWS_PROFILE="eimpresa-automation"
AWS_REGION="eu-west-1"
ECR_REPOSITORY_URI="353153972523.dkr.ecr.eu-west-1.amazonaws.com/authentication-server-dev"
ECS_CLUSTER_NAME="authentication-server-dev"
ECS_SERVICE_NAME="authentication-server-dev"
DOCKER_IMAGE_NAME="authentication-service:latest"

# Get the docker image id
DOCKER_IMAGE_ID="$(docker images -q $DOCKER_IMAGE_NAME)"
test -n "$DOCKER_IMAGE_ID" || (echo "Docker image not found on local repository: $DOCKER_IMAGE_NAME")

echo "aws_profile = $AWS_PROFILE"
echo "aws_region = $AWS_REGION"
echo "ec_repository_uri = $ECR_REPOSITORY_URI"
echo "ecs_cluster_name = $ECS_CLUSTER_NAME"
echo "ecs_service_name = $ECS_SERVICE_NAME"
echo "docker_image_name = $DOCKER_IMAGE_NAME"
echo "docker_image_id = $DOCKER_IMAGE_ID"
echo
read -p "Is this OK? (Yes): " force
[ "$force" = "Yes" ] || (echo "Nothing done, exiting..."; exit 1)

# Get the login command for the ECR Repository
echo "Getting ECR repository credential"
DOCKER_LOGIN_CMD="$(aws ecr get-login --profile $AWS_PROFILE --region $AWS_REGION --no-include-email)"

# Tag the image with the repository url
echo "Tagging repository"
docker tag "$DOCKER_IMAGE_ID" "$ECR_REPOSITORY_URI:latest"

# Push the image to the repository
echo "Pushing the image to ECR"
$DOCKER_LOGIN_CMD
docker push "$ECR_REPOSITORY_URI"

# Remove the image copy created by the taggin operation
docker rmi "$ECR_REPOSITORY_URI:latest"

# Stop running tasks
# tasks=$(\
#     aws ecs list-tasks \
#     --profile "$AWS_PROFILE" --region "$AWS_REGION" \
#     --cluster "$ECS_CLUSTER_NAME" --service "$ECS_SERVICE_NAME" \
#     | jq -r '.taskArns[]'\
# )

# for i in $tasks; do 
#     echo "Stopping task $i"; 
#     aws ecs stop-task \
#     --profile "$AWS_PROFILE" --region "$AWS_REGION" \
#     --cluster "$ECS_CLUSTER_NAME" --task "$i"; 
# done

# Run task
echo "Forcing service redeployment"
aws ecs update-service --profile "$AWS_PROFILE" --region "$AWS_REGION" --cluster "$ECS_CLUSTER_NAME" --service "$ECS_SERVICE_NAME" --force-new-deployment

echo "Deployment triggered"
exit 0