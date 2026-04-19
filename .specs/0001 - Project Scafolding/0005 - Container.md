# 0005 - Container.md

Create the directory `./eng/docker`. Inside this directory, create:

- a `Dockerfile` capable of building and running the API application;
    - This `Dockerfile` must use multistaging build;
    - Must copy only .csproj file and then execute `dotnet restore`;
    - after restore, should copy the entire code;
    - then build and publish application.
    - Use the published version to run the container.
- a `docker-compose` file capable of building the application's Docker image and including:
 - Zipkin
 - Grafana
 - Prometheus
 - Jaeger
- Update the appsettings.Development.json file to use this observability infrastructure via OpenTelemetry.
- Configure a reverse proxy in the docker-compose setup to route traffic to the API running from the container image.

Update the README.md file to include:

- the added tools,
- how they relate to each other,
- how to access and configure them,
- and how to run the entire environment.

Create a `.dockerignore` file, making that only important files to build application will be copied to docker context  