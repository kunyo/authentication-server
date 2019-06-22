#!/bin/sh
set -e

AWSBIN=$(which aws)
PASSBIN=$(which pass)

test -x "$AWSBIN" || (>&2 echo "Missing required package: awscli"; exit 1)
test -x "$PASSBIN" || (>&2 echo "Missing required package: pass"; exit 1)
test -n "$1" || (>&2 echo "Usage: $0 <environment name>"; exit 1)

TARGET_ENV=$1
KMS_KEY_ALIAS="alias/$TARGET_ENV/authentication-service/config-encryption-key"
SIGNING_CREDENTIAL_PATH="./config/$TARGET_ENV/signing-credential/signing-credential.pfx"
WEB_CERTIFICATE_PATH="./config/$TARGET_ENV/$TARGET_ENV-auth.xp3riment.net/$TARGET_ENV-auth.xp3riment.net.pfx"

test -f "$SIGNING_CREDENTIAL_PATH" || (>&2 echo "Certificate not found $SIGNING_CREDENTIAL_PATH"; exit 1)
test -f "$WEB_CERTIFICATE_PATH" || (>&2 echo "Certificate not found $WEB_CERTIFICATE_PATH"; exit 1)
test -n "$AWS_PROFILE" || (>&2 echo "AWS_PROFILE environment variable must be defined."; exit 1)

signing_credential_data=$(cat $SIGNING_CREDENTIAL_PATH | gzip | base64 -w 0)
signing_credential_pwd=$($PASSBIN show /eimpresa/$TARGET_ENV/authentication-service/signing-credential-pwd)

web_certificate_data=$(cat $WEB_CERTIFICATE_PATH | gzip | base64 -w 0)
web_certificate_pwd=$($PASSBIN show /eimpresa/$TARGET_ENV/authentication-service/web-certificate-pwd)

echo "signing-credential-password : $signing_credential_pwd"
echo "web-certificate-password    : $web_certificate_pwd"
read -p "Do you want to continue? (Yes): " reply
if [ "$reply" != "Yes" ]; then
    echo "Nothing done, exiting..."
    exit 1
fi

$AWSBIN ssm put-parameter --overwrite --type "SecureString" --key-id $KMS_KEY_ALIAS --name "/env/$TARGET_ENV/authentication-service/signing-credential-data" --value "$signing_credential_data"
$AWSBIN ssm put-parameter --overwrite --type "SecureString" --key-id $KMS_KEY_ALIAS --name "/env/$TARGET_ENV/authentication-service/signing-credential-password" --value "$signing_credential_pwd"
$AWSBIN ssm put-parameter --overwrite --type "SecureString" --key-id $KMS_KEY_ALIAS --name "/env/$TARGET_ENV/authentication-service/web-certificate-data" --value "$web_certificate_data"
$AWSBIN ssm put-parameter --overwrite --type "SecureString" --key-id $KMS_KEY_ALIAS --name "/env/$TARGET_ENV/authentication-service/web-certificate-password" --value "$web_certificate_pwd"

echo "Put parameters OK"
exit 0