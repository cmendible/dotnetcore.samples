# Setup Couchbase as described here: https://hub.docker.com/r/couchbase/server/

# Pull the couchbase docker image
docker pull couchbase/server

# Run couchbase
docker run -d --name db -p 8091-8094:8091-8094 -p 11210:11210 couchbase

# Visit http://localhost:8091 and setup. Be sure to add the beer-sample bucket.