FROM microsoft/dotnet 

# Set ASPNETCORE_URLS
ENV ASPNETCORE_URLS=https://*:8080

# Switch to root for changing dir ownership/permissions
USER 0

# Copy the binaries
COPY /bin/release/netcoreapp1.1/publish app

# Change to app directory
WORKDIR app

# In order to drop the root user, we have to make some directories world
# writable as OpenShift default security model is to run the container under
# random UID.
RUN chown -R 1001:0 /app && chmod -R og+rwx /app

# Expose port 8080 for the application.
EXPOSE 8080

# Run container by default as user with id 1001 (default)
USER 1001

# Start the application using dotnet!!!
ENTRYPOINT dotnet aspnet.on.openshift.dll