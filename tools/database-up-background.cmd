docker network create restaurant

pushd ..\database
docker compose --verbose up --remove-orphans --wait --detach
popd

pause