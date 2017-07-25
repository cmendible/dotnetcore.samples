dotnet restore
dotnet publish -c release
oc new-app . --name=aspnetoc
oc start-build aspnetoc --from-dir=.
oc create route edge --service=aspnetoc
oc get route aspnetoc