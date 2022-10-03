CREATE TABLE order_event
(
    id uuid NOT NULL,
    aggregate_root_id uuid NOT NULL,
    sequence_number int NOT NULL,
    type text NOT NULL,
    data bytea NOT NULL,
    created_at timestamp NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT ux_order_event__aggregate_root_id_sequence_number UNIQUE (aggregate_root_id, sequence_number)
);

CREATE INDEX ix_order_event__aggregate_root_id ON order_event (aggregate_root_id);