CREATE TABLE t_account_type
(
    id   INTEGER
        constraint t_account_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name TEXT
);

CREATE TABLE t_account
(
    id              INTEGER
        CONSTRAINT t_account_pk
            PRIMARY KEY AUTOINCREMENT,
    name            TEXT,
    account_type_fk INTEGER
        CONSTRAINT t_account_t_account_type_id_fk
            REFERENCES t_account_type
);

CREATE TABLE t_category_type
(
    id   INTEGER
        CONSTRAINT t_category_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name TEXT
);

CREATE TABLE t_mode_payment
(
    id   INTEGER
        CONSTRAINT t_mode_payment_pk
            PRIMARY KEY AUTOINCREMENT,
    name TEXT
);

CREATE TABLE t_place
(
    id         INTEGER
        CONSTRAINT t_place_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT,
    number     TEXT,
    street     TEXT,
    postal     TEXT,
    city       TEXT,
    country    TEXT,
    latitude   REAL,
    longitude  REAL,
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TRIGGER after_insert_on_t_place
    AFTER INSERT ON t_place
    FOR EACH ROW
BEGIN
    UPDATE t_place
    SET date_added = CASE
                   WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                   ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

CREATE TRIGGER after_update_on_t_place
    AFTER UPDATE ON t_place
    FOR EACH ROW
BEGIN
    UPDATE t_place
    SET date_added = CASE
                   WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                   ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TABLE IF EXISTS t_history;
CREATE TABLE t_history
(
    id               INTEGER
        CONSTRAINT t_history_pk
            PRIMARY KEY AUTOINCREMENT,
    compte_fk        INTEGER
        CONSTRAINT t_history_t_account_id_fk
            REFERENCES t_account,
    description          TEXT,
    category_type_fk INTEGER
        CONSTRAINT t_history_t_category_type_id_fk
            REFERENCES t_category_type,
    mode_payment_fk  INTEGER
        CONSTRAINT t_history_t_mode_payment_id_fk
            REFERENCES t_mode_payment,
    value           REAL,
    date             DATETIME,
    place_fk         INTEGER
        constraint t_history_t_place_id_fk
            references t_place,
    pointed BOOLEAN
);

CREATE TRIGGER after_insert_on_t_history
    AFTER INSERT ON t_history
    FOR EACH ROW
BEGIN
    UPDATE t_history
    SET date = CASE
                   WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                   ELSE NEW.date
        END
    WHERE id = NEW.id;
END;

CREATE TRIGGER after_update_on_t_history
    AFTER UPDATE ON t_history
    FOR EACH ROW
BEGIN
    UPDATE t_history
    SET date = CASE
                   WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                   ELSE NEW.date
        END
    WHERE id = NEW.id;
END;
