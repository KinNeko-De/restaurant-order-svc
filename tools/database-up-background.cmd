docker network create restaurant-dev-net

pushd ..\database
docker-compose --verbose up --remove-orphans --wait --detach
popd

pause