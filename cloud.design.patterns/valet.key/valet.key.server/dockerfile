# We use the microsoft/dotnet image as a starting point.
FROM microsoft/dotnet

# Install git
RUN apt-get install git -y

# Create a folder to clone our source code 
RUN mkdir repositories

# Set our working folder
WORKDIR repositories

# Clone the source code
RUN git clone https://github.com/cmendible/dotnetcore.samples.git

# Set our working folder
WORKDIR dotnetcore.samples/cloud.design.patterns/valet.key/valet.key.server

# Expose port 5000 for the application.
EXPOSE 5000

# Restore nuget packages
RUN dotnet restore

# Restore nuget packages
RUN dotnet restore

# Start the application using dotnet!!!
ENTRYPOINT dotnet run