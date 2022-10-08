# Database
This project uses a postgreSQL database.

## Patches
All patches are deployed and versioned using [Flyway](https://flywaydb.org/).

Patching is done by the microservice that is the owner of the database.

### Create SQL patch
Use the [CMD](create-new-patch.cmd) to create a unique file name. Insert your sql statemtent into it.

## Database for local setup
The following command starts a postgres db with an patched database:
`docker-compose up`

To enter the psql command shell:
`docker exec -it restaurant-order-db psql -U orderuser -d orderdatabase`

To shut the database down and delete all data use:
`docker-compose down`

There are scripts for easy usageare on top level under the folder [tools](../tools).

## Automated component tests with database
If you want to start the component tests on your local setup, you have to start the database before you run the test.
