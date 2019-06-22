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

## Test

### Unit Tests
```
./run-tests.sh
```

### Integration Tests
```
AWS_PROFILE=eimpresa-automation \
AWS_REGION=eu-west-1 \
TEST_CONFIG=$(pwd)/config/dev/appsettings.json \
./run-tests.sh *.IntegrationTests.csproj
```

### Smoke Tests
```
TEST_CONFIG=$(pwd)/config/local/appsettings.local.json \
./run-tests.sh *.SmokeTests.csproj
```


## Configuration

#### 1. Create a certification authority.
```
cd scripts
./new-ca.sh root-ca
```

#### 2. Add the certification authority to the trusted certificates dir
On Debian hosts:
```
sudo cp root-ca.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
```
On CentOS hosts:
```
sudo cp root-ca.crt /etc/pki/ca-trust/source/anchors/
sudo update-ca-trust extract
```

#### 3. Create a self signed certificate used to sign the tokens
```
cd scripts
./new-pkcs12-certificate.sh --common-name=signing-credential --self-signed
```

#### 4. Create a ssl certificate for the webserver
```
./new-pkcs12-certificate.sh --common-name=local-auth.xp3riment.net --signing-key=./root-ca/root-ca.key --signing-cert=./root-ca/root-ca.crt
```

#### 5. Copy the newly created certificates to the web content directory

```
cp ./local-api.xp3riment.net/local-api.xp3riment.net.pfx ./../src/AuthenticationService/

cp ./signing-credential/signing-credential.pfx ./../src/AuthenticationService/
```