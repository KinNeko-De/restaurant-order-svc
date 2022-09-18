docker network create restaurant-dev-net

pushd ..\database
docker-compose --verbose up --remove-orphans
popd

pause