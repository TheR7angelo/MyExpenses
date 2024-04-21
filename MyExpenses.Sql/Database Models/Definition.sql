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
    AFTER INSERT
    ON t_place
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
    AFTER UPDATE
    ON t_place
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
    description      TEXT,
    category_type_fk INTEGER
        CONSTRAINT t_history_t_category_type_id_fk
            REFERENCES t_category_type,
    mode_payment_fk  INTEGER
        CONSTRAINT t_history_t_mode_payment_id_fk
            REFERENCES t_mode_payment,
    value            REAL,
    date             DATETIME DEFAULT CURRENT_TIMESTAMP,
    place_fk         INTEGER
        constraint t_history_t_place_id_fk
            references t_place,
    pointed          BOOLEAN  DEFAULT FALSE
);

CREATE TRIGGER after_insert_on_t_history
    AFTER INSERT
    ON t_history
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
    AFTER UPDATE
    ON t_history
    FOR EACH ROW
BEGIN
    UPDATE t_history
    SET date = CASE
                   WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                   ELSE NEW.date
        END
    WHERE id = NEW.id;
END;

DROP VIEW IF EXISTS v_history;
CREATE VIEW v_history AS
SELECT ta.name  AS account,
       h.description,
       tct.name AS category,
       tmp.name AS mode_payment,
       h.value,
       h.date,
       tp.name AS place,
       h.pointed

FROM t_history h
         LEFT JOIN t_account ta
                   ON h.compte_fk = ta.id
         LEFT JOIN t_category_type tct
                   ON h.category_type_fk = tct.id
         LEFT JOIN t_mode_payment tmp
                   ON h.mode_payment_fk = tmp.id
         LEFT JOIN t_place tp
             ON h.place_fk = tp.id;

-- DROP VIEW IF EXISTS v_value_by_month_year;
-- CREATE VIEW v_value_by_month_year AS
-- SELECT CAST(STRFTIME('%Y', h.date) AS INTEGER) AS year,
--        CAST(STRFTIME('%m', h.date) AS INTEGER) AS month,
--        h.compte_fk,
--        ROUND(SUM(h.value), 2)                  AS total
-- FROM t_history h
-- GROUP BY year, month, h.compte_fk
-- ORDER BY year, month;
--
-- DROP VIEW IF EXISTS v_value_by_month_year_category;
-- CREATE VIEW v_value_by_month_year_category AS
-- SELECT CAST(STRFTIME('%Y', h.date) AS INTEGER) AS year,
--        CAST(STRFTIME('%m', h.date) AS INTEGER) AS month,
--        ct.name,
--        IFNULL(ROUND(SUM(h.value), 2), 0)                  AS total
-- FROM t_category_type ct
--          LEFT JOIN t_history h
--                    ON ct.id = h.category_type_fk
-- GROUP BY year, month, ct.name
-- ORDER BY year, month;

CREATE VIEW v_total_category AS
SELECT strftime('%Y', h.date) AS year,
       strftime('%m', h.date) AS month,
       tct.name               AS category,
       ROUND(SUM(h.value), 2) AS value

FROM t_category_type tct
         LEFT JOIN t_history h
                   ON h.category_type_fk = tct.id
GROUP BY year, month, tct.name;


