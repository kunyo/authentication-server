#!/bin/sh
set -ex
WORKSPACE=$(realpath $(dirname $0))
KEY_DIR=$WORKSPACE/keys
EXPIRES_IN=31
test -n "$1" || (echo "Usage: $0 <key name>"; exit 1)
test -n "$EXPIRES_IN" || (echo "EXPIRES_IN not defined"; exit 1)
test -d "$KEY_DIR/$1" && (echo "Key already exists: $1"; exit 1)

mkdir -p "$KEY_DIR/$1"
cd "$KEY_DIR/$1"
openssl req -nodes -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days $EXPIRES_IN
openssl pkcs12 -export -out cert.pfx -inkey key.pem -in cert.pem
echo "To deploy the key to the development copy run the following command:"
echo "cp $(realpath cert.pfx) \"$(realpath $WORKSPACE/../src/AuthenticationService)\""
exit 0