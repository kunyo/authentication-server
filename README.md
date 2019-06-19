# Authentication server

## Requirements
Identity server
dotnetcore 2.1

## Install
Pull latest release from github

## Run
The service runs into a Docker container. To deploy to your local Docker server run:
```
./deploy.sh
```

## Configuration

#### 1. Create a certification authority.
```
cd scripts
./new-ca.sh root-ca
```

#### 2. Create a self signed certificate used to sign the tokens
```
cd scripts
./new-pkcs12-certificate.sh --common-name=signing-credential --self-signed
```

#### 3. Create a ssl certificate for the webserver
```
./new-pkcs12-certificate.sh --common-name=local-api.xp3riment.net --signing-key=./root-ca/root-ca.key --signing-cert=./root-ca/root-ca.crt
```

#### 4. Copy the newly created certificates to the web content directory

```
cp ./local-api.xp3riment.net/local-api.xp3riment.net.pfx ./../src/AuthenticationService/

cp ./signing-credential/signing-credential.pfx ./../src/AuthenticationService/
```