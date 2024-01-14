:: starts the system under test
docker network create restaurant

pushd ..\restaurant.svcorder
dotnet publish -o ../sut/publish -c Release --no-self-contained
popd

pushd ..\sut
docker compose --verbose up --build --remove-orphans --wait --detach
popd

pause
