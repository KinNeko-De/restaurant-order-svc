CREATE TABLE order_relation
(
    id uuid NOT NULL,
    sequence_number int NOT NULL,
    record_created_at timestamptz NOT NULL,
    record_updated_at timestamptz,
    PRIMARY KEY (id)
);