CREATE TABLE order_relation
(
    id uuid NOT NULL,
    sequence_number int NOT NULL,
    record_created_at timestamp NOT NULL,
    record_updated_at timestamp,
    PRIMARY KEY (id)
);