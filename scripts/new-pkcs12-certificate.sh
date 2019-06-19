#!/bin/sh
set -e

usage () {
    cat <<EOF
Certificate generation utility     
Usage: $0 --self--signed | --key=<root ca key> --common-name=<CN> [options]

Accepted Options
--self-signed   Issue a self signed certificate
--common-name   The CN field of the Certificate Signing Request
--signing-key   The root key to use to sign the request
--signing-cert  The root certificate targeted by the request
--subject-name  The SN fields of the Certificate Signing Request

EOF
    exit 1
}

. "$(dirname $0)/vars.sh"

SELF_SIGNED=0
CA_CERT_FILE=
CA_KEY_FILE=
export KEY_CN=
export KEY_NAME=

for i in "$@"
do
    case "$i" in
        --self-signed) 
            SELF_SIGNED=1 ;;
        --common-name=*)
            export KEY_CN="${i#*=}" ;;
        --subject-name=*)
            export KEY_NAME="${i#*=}" ;;            
        --signing-cert=*)
            CA_CERT_FILE=$(realpath "${i#*=}") ;;            
        --signing-key=*)
            CA_KEY_FILE=$(realpath "${i#*=}") ;;
        -h|--help) 
            usage ;;
        *) 
            echo "Invalid option: $i. To see the list of accepted options use $0 --help"; 
            exit 1 ;;
    esac
done

if [ "$SELF_SIGNED" -eq 0 ] && [ "$CA_KEY_FILE" = "" ]; then
    echo "You must define either the --self-signed or the --signing-cert and --signing-key option"
    exit 1
fi

if [ "$SELF_SIGNED" -eq 1 ] && [ "$CA_KEY_FILE" != "" ]; then
    echo "--self-signed cannot be used in conjunction with --signing-cert or --signing-key"
    exit 1
fi

test -n "$KEY_DIR" || (echo 'KEY_DIR is not defined'; exit 1)
test -n "$KEY_CN" || (echo '--common-name option must be defined'; exit 1)
test -d "$KEY_DIR/$KEY_CN" && (echo "Key already exists: $KEY_CN"; exit 1)

# Make sure both the root keys and certificate exist
if [ "$SELF_SIGNED" -eq 0 ]; then
    test -f "$CA_CERT_FILE" || (echo "--signing-cert: file not found"; exit 1)
    test -f "$CA_KEY_FILE" || (echo "--signing-key: file not found"; exit 1)
fi

mkdir -p "$KEY_DIR/$KEY_CN"
cd "$KEY_DIR/$KEY_CN"

if [ "$SELF_SIGNED" -eq 1 ]; then
    echo "Creating a self signed certificate"

    # Create a root key
    openssl genrsa -out "$KEY_CN.key" $KEY_SIZE
    KEY_FILE=$(realpath "$KEY_CN.key")

    # Creates a self signed x509 certificate
    openssl req -batch -nodes -x509 -key "$KEY_CN.key" -out "$KEY_CN.crt" -days $KEY_EXPIRE
else

    # Create a root key
    openssl genrsa -out "$KEY_CN.key" $KEY_SIZE
    KEY_FILE=$(realpath "$KEY_CN.key")

    # Creates certificate signing request
    openssl req -new -batch -nodes -key "$KEY_CN.key" -out "$KEY_CN.csr" 
    
    # Signin the request using the root certificate
    openssl x509 -req -in "$KEY_CN.csr" -CA $CA_CERT_FILE -CAkey $CA_KEY_FILE -CAcreateserial -out "$KEY_CN.crt" -days $KEY_EXPIRE -sha256
fi

# Combine the RSA private key and the x509 certificate into a pkcs12 certificate
openssl pkcs12 -export -inkey "$KEY_CN.key" -in "$KEY_CN.crt" -out "$KEY_CN.pfx"    

echo "Certificate created"
exit 0