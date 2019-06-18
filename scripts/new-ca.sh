#!/bin/sh
set -ex

test -n "$1" || (echo "Usage: $0 <common name> [<subject name>]"; exit 1)

. "$(dirname $0)/vars.sh"

test -n "$KEY_DIR" 
test -d "$KEY_DIR/$1" && (echo "Key already exists: $1"; exit 1)

export KEY_CN=$1
export KEY_NAME=$2

mkdir -p "$KEY_DIR/$KEY_CN"
cd "$KEY_DIR/$KEY_CN"

# Create an RSA 2048 bit private key
# The CA root key must be kept secret.
#openssl genrsa -des3 -out "$KEY_CN.key" $KEY_SIZE
openssl genrsa -out "$KEY_CN.key" $KEY_SIZE

# Creates a self signed x509 certificate
openssl req -batch -x509 -new -nodes -key "$KEY_CN.key" -out "$KEY_CN.pem" -days $KEY_EXPIRE

echo "CA created"
exit 0