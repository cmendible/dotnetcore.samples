FROM microsoft/azure-functions-runtime:2.0.0-jessie

# If we don't set WEBSITE_HOSTNAME Azure Function triggers other than HttpTrigger won't run. (FunctionInvocationException: Webhooks are not configured)
ENV WEBSITE_HOSTNAME=localhost:80

# Copy all Function files and binaries to /home/site/wwwroot
ADD wwwroot /home/site/wwwroot

# Functions must live in: /home/site/wwwroot
# We expect to receive the Storage Account Connection String from the command line.
ARG STORAGE_ACCOUNT

# Set the AzureWebJobsStorage Environment Variable. Otherwise Durable Functions Extensions won't work. 
ENV AzureWebJobsStorage=$STORAGE_ACCOUNT