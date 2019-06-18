#!/bin/sh
set -e

test -n "$1" || (echo "Usage: $0 <docker image id>"; exit 1)

AWS_PROFILE="eimpresa-automation"
AWS_REGION="eu-west-1"
ECR_REPOSITORY_URI="353153972523.dkr.ecr.eu-west-1.amazonaws.com/authentication-server-dev"
ECS_CLUSTER_NAME="authentication-server-dev"
ECS_SERVICE_NAME="authentication-server-dev"
TASK_DEFINITION_FAMILY="authentication-server-dev"

# Get the login command for the ECR Repository
echo "Getting ECR repository credential"
DOCKER_LOGIN_CMD="$(aws ecr get-login --profile $AWS_PROFILE --region $AWS_REGION --no-include-email)"

# Tag the image with the repository url
echo "Tagging repository"
docker tag "$1" "$ECR_REPOSITORY_URI:latest"

# Push the image to the repository
echo "Pushing the image to ECR"
$DOCKER_LOGIN_CMD
docker push "$ECR_REPOSITORY_URI"

# Remove the image copy created by the taggin operation
docker rmi "$ECR_REPOSITORY_URI:latest"

# Stop running tasks
tasks=$(\
    aws ecs list-tasks \
    --profile "$AWS_PROFILE" --region "$AWS_REGION" \
    --cluster "$ECS_CLUSTER_NAME" --service "$ECS_SERVICE_NAME" \
    | jq -r '.taskArns[]'\
)

for i in $tasks; do 
    echo "Stopping task $i"; 
    aws ecs stop-task \
    --profile "$AWS_PROFILE" --region "$AWS_REGION" \
    --cluster "$ECS_CLUSTER_NAME" --task "$i"; 
done

# Run task
echo "Forcing service redeployment"
aws ecs update-service --profile "$AWS_PROFILE" --region "$AWS_REGION" --cluster "$ECS_CLUSTER_NAME" --service "$ECS_SERVICE_NAME" --force-new-deployment

echo "Deployment triggered"
exit 0