-- region Tables

DROP TABLE IF EXISTS t_account_type;
CREATE TABLE t_account_type
(
    id         INTEGER
        constraint t_account_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT,
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_currency;
CREATE TABLE t_currency
(
    id         INTEGER
        CONSTRAINT t_account_pk
            PRIMARY KEY AUTOINCREMENT,
    symbol     TEXT,
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_account;
CREATE TABLE t_account
(
    id              INTEGER
        CONSTRAINT t_account_pk
            PRIMARY KEY AUTOINCREMENT,
    name            TEXT,
    account_type_fk INTEGER
        CONSTRAINT t_account_t_account_type_id_fk
            REFERENCES t_account_type,
    currency_fk     INTEGER
        constraint t_account_t_currency_id_fk
            references t_currency,
    active          BOOLEAN  DEFAULT TRUE,
    date_added      DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_color;
CREATE TABLE t_color
(
    id                     INTEGER
        constraint t_account_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name                   TEXT,
    hexadecimal_color_code TEXT(9),
    date_added             DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_category_type;
CREATE TABLE t_category_type
(
    id         INTEGER
        CONSTRAINT t_category_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT,
    color_fk   integer
        constraint t_account_type_t_color_id_fk
            references t_color,
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_mode_payment;
CREATE TABLE t_mode_payment
(
    id         INTEGER
        CONSTRAINT t_mode_payment_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT,
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_place;
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

DROP TABLE IF EXISTS t_bank_transfer;
CREATE TABLE t_bank_transfer
(
    id                INTEGER
        CONSTRAINT t_bank_transfer_pk
            PRIMARY KEY AUTOINCREMENT,
    value             REAL,
    from_account_fk   INTEGER
        CONSTRAINT t_bank_transfer_t_account_id_fk
            REFERENCES t_account,
    to_account_fk        INTEGER
        CONSTRAINT t_bank_transfer_t_account_id_fk_2
            REFERENCES t_account,
    main_reason       TEXT,
    additional_reason TEXT,
    date              DATETIME,
    date_added        DATETIME default CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_history;
CREATE TABLE t_history
(
    id               INTEGER
        CONSTRAINT t_history_pk
            PRIMARY KEY AUTOINCREMENT,
    account_fk       INTEGER
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
    date             DATETIME,
    place_fk         INTEGER
        constraint t_history_t_place_id_fk
            references t_place,
    pointed          BOOLEAN  DEFAULT FALSE,
    bank_transfer_fk INTEGER
        CONSTRAINT t_history_t_bank_transfer_id_fk
            REFERENCES t_bank_transfer,
    date_added       DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- endregion

-- region Triggers
DROP TRIGGER IF EXISTS after_insert_on_t_account_type;
CREATE TRIGGER after_insert_on_t_account_type
    AFTER INSERT
    ON t_account_type
    FOR EACH ROW
BEGIN
    UPDATE t_account_type
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_account_type;
CREATE TRIGGER after_update_on_t_account_type
    AFTER UPDATE
    ON t_account_type
    FOR EACH ROW
BEGIN
    UPDATE t_account_type
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_insert_on_t_currency;
CREATE TRIGGER after_insert_on_t_currency
    AFTER INSERT
    ON t_currency
    FOR EACH ROW
BEGIN
    UPDATE t_currency
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_currency;
CREATE TRIGGER after_update_on_t_currency
    AFTER UPDATE
    ON t_currency
    FOR EACH ROW
BEGIN
    UPDATE t_currency
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_insert_on_t_color;
CREATE TRIGGER after_insert_on_t_color
    AFTER INSERT
    ON t_color
    FOR EACH ROW
BEGIN
    UPDATE t_color
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_color;
CREATE TRIGGER after_update_on_t_color
    AFTER UPDATE
    ON t_color
    FOR EACH ROW
BEGIN
    UPDATE t_color
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_insert_on_t_category_type;
CREATE TRIGGER after_insert_on_t_category_type
    AFTER INSERT
    ON t_category_type
    FOR EACH ROW
BEGIN
    UPDATE t_category_type
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_account_type;
CREATE TRIGGER after_update_on_t_account_type
    AFTER UPDATE
    ON t_category_type
    FOR EACH ROW
BEGIN
    UPDATE t_category_type
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_insert_on_t_account;
CREATE TRIGGER after_insert_on_t_account
    AFTER INSERT
    ON t_account
    FOR EACH ROW
BEGIN
    UPDATE t_account
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_account;
CREATE TRIGGER after_update_on_t_account
    AFTER UPDATE
    ON t_account
    FOR EACH ROW
BEGIN
    UPDATE t_account
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_insert_on_t_mode_payment;
CREATE TRIGGER after_insert_on_after_insert_on_t_mode_payment
    AFTER INSERT
    ON t_mode_payment
    FOR EACH ROW
BEGIN
    UPDATE t_mode_payment
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_mode_payment;
CREATE TRIGGER after_update_on_t_mode_payment
    AFTER UPDATE
    ON t_mode_payment
    FOR EACH ROW
BEGIN
    UPDATE t_mode_payment
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

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

DROP TRIGGER IF EXISTS after_insert_on_t_history;
CREATE TRIGGER after_insert_on_t_history
    AFTER INSERT
    ON t_history
    FOR EACH ROW
BEGIN
    UPDATE t_history
    SET date       = CASE
                         WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                         ELSE NEW.date
        END,
        date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
            END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_history;
CREATE TRIGGER after_update_on_t_history
    AFTER UPDATE
    ON t_history
    FOR EACH ROW
BEGIN
    UPDATE t_history
    SET date       = CASE
                         WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                         ELSE NEW.date
        END,
        date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
            END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_insert_on_t_bank_transfer;
CREATE TRIGGER after_insert_on_t_bank_transfer
    AFTER INSERT
    ON t_bank_transfer
    FOR EACH ROW
BEGIN
    UPDATE t_bank_transfer
    SET date       = CASE
                         WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                         ELSE NEW.date
        END,
        date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
            END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_bank_transfer;
CREATE TRIGGER after_update_on_t_bank_transfer
    AFTER UPDATE
    ON t_bank_transfer
    FOR EACH ROW
BEGIN
    UPDATE t_bank_transfer
    SET date       = CASE
                         WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                         ELSE NEW.date
        END,
        date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
            END
    WHERE id = NEW.id;
END;

-- endregion

-- region Views

DROP VIEW IF EXISTS v_history;
CREATE VIEW v_history AS
SELECT h.id,
       ta.name  AS account,
       h.description,
       tct.name AS category,
       tmp.name AS mode_payment,
       h.value,
       tc.symbol,
       h.date,
       tp.name  AS place,
       h.pointed,
       bt.main_reason,
       h.date_added

FROM t_history h
         LEFT JOIN t_account ta
                   ON h.account_fk = ta.id
         LEFT JOIN t_category_type tct
                   ON h.category_type_fk = tct.id
         LEFT JOIN t_mode_payment tmp
                   ON h.mode_payment_fk = tmp.id
         LEFT JOIN t_currency tc
                   ON ta.currency_fk = tc.id
         LEFT JOIN t_place tp
                   ON h.place_fk = tp.id
         LEFT JOIN t_bank_transfer bt
                   ON h.bank_transfer_fk = bt.id;

-- DROP VIEW IF EXISTS v_value_by_month_year;
-- CREATE VIEW v_value_by_month_year AS
-- SELECT CAST(STRFTIME('%Y', h.date) AS INTEGER) AS year,
--        CAST(STRFTIME('%m', h.date) AS INTEGER) AS month,
--        h.account_fk,
--        ROUND(SUM(h.value), 2)                  AS total
-- FROM t_history h
-- GROUP BY year, month, h.account_fk
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

DROP VIEW IF EXISTS v_total_by_account;
CREATE VIEW v_total_by_account AS
SELECT ta.id,
       ta.name,
       ROUND(SUM(th.value), 2) AS total,
       tc.symbol
FROM t_account ta
         LEFT JOIN t_history th
                   ON ta.id = th.account_fk
         LEFT JOIN t_currency tc
                   ON ta.currency_fk = tc.id
GROUP BY ta.id, ta.name, tc.symbol
ORDER BY ta.id;

DROP VIEW IF EXISTS v_detail_total_category;
CREATE VIEW v_detail_total_category AS
SELECT CAST(STRFTIME('%Y', h.date) AS INT) AS year,
       CAST(STRFTIME('%W', h.date) AS INT) AS week,
       CAST(STRFTIME('%m', h.date) AS INT) AS month,
       CAST(STRFTIME('%d', h.date) AS INT) AS day,
       ta.name                             AS account,
       tct.name                            AS category,
       h.value,
       tcu.symbol,
       tco.hexadecimal_color_code

FROM t_category_type tct
         LEFT JOIN t_history h
                   ON h.category_type_fk = tct.id
         LEFT JOIN t_account ta
                   ON h.account_fk = ta.id
         LEFT JOIN t_currency tcu
                   ON ta.currency_fk = tcu.id
         LEFT JOIN t_color tco
                   ON tct.color_fk = tco.id
ORDER BY year, week;

DROP VIEW IF EXISTS v_bank_transfer;
CREATE VIEW v_bank_transfer AS
SELECT bk.id,
       bk.value,
       fa.name AS from_account,
       ta.name AS to_account,
       bk.main_reason,
       bk.additional_reason,
       bk.date,
       bk.date_added
FROM t_bank_transfer bk
         INNER JOIN t_account fa
                    ON bk.from_account_fk = fa.id
         INNER JOIN t_account ta
            ON bk.to_account_fk = ta.id;

-- endregion


