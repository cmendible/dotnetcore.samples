FROM microsoft/dotnet 

# Install git
RUN apt-get install git -y

# Clone the source code
RUN git clone -b ssl https://github.com/cmendible/aspnet-core-helloworld.git

# Set our working folder
WORKDIR aspnet-core-helloworld/src/dotnetstarter

# Restore nuget packages
RUN dotnet restore

# Build the application using dotnet!!!
RUN dotnet build

# Set password for the certificate as 1234
# I'm using Environment Variable here to simplify the .NET Core sample.
ENV certPassword 1234

# Use opnssl to generate a self signed certificate cert.pfx with password $env:certPassword
RUN openssl genrsa -des3 -passout pass:${certPassword} -out server.key 2048
RUN openssl rsa -passin pass:${certPassword} -in server.key -out server.key
RUN openssl req -sha256 -new -key server.key -out server.csr -subj '/CN=localhost'
RUN openssl x509 -req -sha256 -days 365 -in server.csr -signkey server.key -out server.crt
RUN openssl pkcs12 -export -out cert.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:${certPassword}

# Expose port 443 for the application.
EXPOSE 443

# Start the application using dotnet!!!
ENTRYPOINT dotnet run