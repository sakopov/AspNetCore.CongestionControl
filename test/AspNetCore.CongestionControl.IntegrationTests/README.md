# Integration Tests

This project verifies functionality of congestion control component by using a test API and running tests against it. Note, a Redis instance must be installed and running prior to running tests.

Execute `dotnet test` to run tests.

## Redis Configuration

A Redis instance needs to be installed and running bound to port 6379. The following steps install Redis container using Docker.

1. Install [Docker](https://docs.docker.com/get-docker/).
2. Execute `npm install -g redis-cli` to install redis-cli.
3. Execute `docker pull redis` to get latest Redis image.
4. Execute `docker run --name redis -d -p 6379:6379 redis` to start Redis bound to port 6379.
5. Execute `rdcli` to verify connectivity with Redis instance.
