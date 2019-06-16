#/!bin/sh
set -e
set -e
cd "$(dirname $0)"
docker build -t 'eimpresa/authentication-service' .
echo "Build OK"
exit 0