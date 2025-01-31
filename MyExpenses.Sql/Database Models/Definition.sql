-- region Tables

DROP TABLE IF EXISTS t_version;
CREATE TABLE t_version
(
    id      INTEGER
        CONSTRAINT t_version_pk
            PRIMARY KEY AUTOINCREMENT,
    version TEXT
);

DROP TABLE IF EXISTS t_supported_languages;
CREATE TABLE t_supported_languages
(
    id               INTEGER
        CONSTRAINT t_supported_languages_pk
            PRIMARY KEY AUTOINCREMENT,
    code             TEXT(10)    NOT NULL
        CONSTRAINT t_supported_languages_pk_2
            UNIQUE,
    native_name      TEXT(55)    NOT NULL,
    english_name     TEXT(55)    NOT NULL,
    default_language BOOLEAN NOT NULL DEFAULT FALSE,
    date_added       DATETIME         DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_spatial_ref_sys;
CREATE TABLE t_spatial_ref_sys
(
    srid      INTEGER not null
        constraint t_spatial_ref_sys_pk
            primary key,
    auth_name TEXT(50)    not null,
    auth_srid TEXT(10)    not null,
    srtext    TEXT(2000)    not null,
    proj4text TEXT(255)    not null,
    constraint t_spatial_ref_sys_srid_check
        check (srid > 0 AND srid <= 998999)
);

DROP TABLE IF EXISTS t_geometry_columns;
CREATE TABLE t_geometry_columns
(
    id                INTEGER
        constraint t_geometry_columns_pk
            primary key autoincrement,
    f_table_name      TEXT    not null,
    f_geometry_column TEXT    not null,
    type              TEXT    not null,
    coord_dimension   INTEGER not null,
    srid              INTEGER not null
        constraint t_geometry_columns_t_spatial_ref_sys_srid_fk
            references t_spatial_ref_sys
);

DROP TABLE IF EXISTS t_account_type;
CREATE TABLE t_account_type
(
    id         INTEGER
        constraint t_account_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT(100),
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_currency;
CREATE TABLE t_currency
(
    id         INTEGER
        CONSTRAINT t_account_pk
            PRIMARY KEY AUTOINCREMENT,
    symbol     TEXT(55),
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_account;
CREATE TABLE t_account
(
    id              INTEGER
        CONSTRAINT t_account_pk
            PRIMARY KEY AUTOINCREMENT,
    name            TEXT(55),
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
    name                   TEXT(55),
    hexadecimal_color_code TEXT(9),
    date_added             DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_category_type;
CREATE TABLE t_category_type
(
    id         INTEGER
        CONSTRAINT t_category_type_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT(55),
    color_fk   integer
        constraint t_account_type_t_color_id_fk
            references t_color,
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
    to_account_fk     INTEGER
        CONSTRAINT t_bank_transfer_t_account_id_fk_2
            REFERENCES t_account,
    main_reason       TEXT(100),
    additional_reason TEXT(255),
    date              DATETIME,
    date_added        DATETIME default CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_recursive_frequency;
CREATE TABLE t_recursive_frequency
(
    id          INTEGER
        CONSTRAINT t_recursive_frequency_pk
            PRIMARY KEY AUTOINCREMENT,
    frequency   TEXT(55),
    description TEXT(100)
);

DROP TABLE IF EXISTS t_mode_payment;
CREATE TABLE t_mode_payment
(
    id         INTEGER
        CONSTRAINT t_mode_payment_pk
            PRIMARY KEY AUTOINCREMENT,
    name       TEXT(55),
    date_added DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_place;
CREATE TABLE t_place
(
    id             INTEGER
        CONSTRAINT t_place_pk
            PRIMARY KEY AUTOINCREMENT,
    name           TEXT(155),
    number         TEXT(20),
    street         TEXT(155),
    postal         TEXT(10),
    city           TEXT(100),
    country        TEXT(55),
    latitude       REAL,
    longitude      REAL,
    is_open        BOOLEAN NOT NULL DEFAULT TRUE,
    can_be_deleted BOOLEAN DEFAULT TRUE,
    date_added     DATETIME DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_recursive_expense;
CREATE TABLE t_recursive_expense
(
    id               INTEGER
        CONSTRAINT t_recursive_expense_pk
            PRIMARY KEY AUTOINCREMENT,
    account_fk       INTEGER
        CONSTRAINT t_recursive_expense_t_account_id_fk
            REFERENCES t_account,
    description      TEXT,
    note             TEXT,
    category_type_fk INTEGER
        CONSTRAINT t_recursive_expense_t_category_type_id_fk
            REFERENCES t_category_type,
    mode_payment_fk  INTEGER
        CONSTRAINT t_recursive_expense_t_mode_payment_id_fk
            REFERENCES t_mode_payment,
    value            REAL,
    place_fk         INTEGER  DEFAULT 0
        CONSTRAINT t_recursive_expense_t_place_id_fk
            REFERENCES t_place,
    start_date       DATE                   NOT NULL,
    recursive_total  INTEGER,
    recursive_count  INTEGER  DEFAULT 0     NOT NULL,
    frequency_fk     INTEGER                NOT NULL
        CONSTRAINT t_recursive_expense_t_recursive_frequency_id_fk
            REFERENCES t_recursive_frequency,
    next_due_date    DATE                   NOT NULL,
    is_active        BOOLEAN  DEFAULT TRUE  NOT NULL,
    force_deactivate BOOLEAN  DEFAULT FALSE NOT NULL,
    date_added       DATETIME DEFAULT CURRENT_TIMESTAMP,
    last_updated     DATETIME
);

DROP TABLE IF EXISTS t_history;
CREATE TABLE t_history
(
    id                   INTEGER
        CONSTRAINT t_history_pk
            PRIMARY KEY AUTOINCREMENT,
    account_fk           INTEGER
        CONSTRAINT t_history_t_account_id_fk
            REFERENCES t_account,
    description          TEXT(255),
    category_type_fk     INTEGER
        CONSTRAINT t_history_t_category_type_id_fk
            REFERENCES t_category_type,
    mode_payment_fk      INTEGER
        CONSTRAINT t_history_t_mode_payment_id_fk
            REFERENCES t_mode_payment,
    value                REAL,
    date                 DATETIME,
    place_fk             INTEGER
        constraint t_history_t_place_id_fk
            references t_place,
    is_pointed              BOOLEAN NOT NULL DEFAULT FALSE,
    bank_transfer_fk     INTEGER
        CONSTRAINT t_history_t_bank_transfer_id_fk
            REFERENCES t_bank_transfer,
    recursive_expense_fk INTEGER
        CONSTRAINT t_history_t_recursive_expense_id_fk
            REFERENCES t_recursive_expense,
    date_added           DATETIME DEFAULT CURRENT_TIMESTAMP,
    date_pointed         DATETIME
);

-- endregion

-- region Triggers
DROP TRIGGER IF EXISTS after_insert_on_t_supported_languages;
CREATE TRIGGER after_insert_on_t_supported_languages
    AFTER INSERT
    ON t_supported_languages
    FOR EACH ROW
BEGIN
    UPDATE t_supported_languages
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_supported_languages;
CREATE TRIGGER after_update_on_t_supported_languages
    AFTER UPDATE
    ON t_supported_languages
    FOR EACH ROW
BEGIN
    UPDATE t_supported_languages
    SET date_added = CASE
                         WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                         ELSE NEW.date_added
        END
    WHERE id = NEW.id;
END;

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

DROP TRIGGER IF EXISTS after_insert_on_t_place;
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

DROP TRIGGER IF EXISTS after_update_on_t_place;
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
    SET date         = CASE
                           WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                           ELSE NEW.date
        END,
        date_added   = CASE
                           WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                           ELSE NEW.date_added
            END,
        date_pointed = CASE
                           WHEN NEW.is_pointed = 1 AND typeof(NEW.date) = 'integer'
                               THEN datetime(NEW.date / 1000, 'unixepoch')
                           WHEN NEW.is_pointed = 0 THEN NULL
                           ELSE date_pointed
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
    SET date         = CASE
                           WHEN typeof(NEW.date) = 'integer' THEN datetime(NEW.date / 1000, 'unixepoch')
                           ELSE NEW.date
        END,
        date_added   = CASE
                           WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                           ELSE NEW.date_added
            END,
        date_pointed = CASE
                           WHEN NEW.is_pointed = 1 AND typeof(NEW.date) = 'integer'
                               THEN datetime(NEW.date / 1000, 'unixepoch')
                           WHEN NEW.is_pointed = 0 THEN NULL
                           ELSE date_pointed
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

DROP TRIGGER IF EXISTS after_insert_on_t_recursive_expense;
CREATE TRIGGER after_insert_on_t_recursive_expense
    AFTER INSERT
    ON t_recursive_expense
    FOR EACH ROW
BEGIN
    UPDATE t_recursive_expense
    SET start_date    = CASE
                            WHEN typeof(NEW.start_date) = 'integer' THEN date(NEW.start_date / 1000, 'unixepoch')
                            ELSE NEW.start_date
        END,
        next_due_date = CASE
                            WHEN typeof(NEW.next_due_date) = 'integer'
                                THEN date(NEW.next_due_date / 1000, 'unixepoch')
                            ELSE NEW.next_due_date
            END,
        date_added    = CASE
                            WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                            ELSE NEW.date_added
            END
    WHERE id = NEW.id;
END;

DROP TRIGGER IF EXISTS after_update_on_t_recursive_expense;
CREATE TRIGGER after_update_on_t_recursive_expense
    AFTER UPDATE
    ON t_recursive_expense
    FOR EACH ROW
BEGIN
    UPDATE t_recursive_expense
    SET start_date    = CASE
                            WHEN typeof(NEW.start_date) = 'integer' THEN date(NEW.start_date / 1000, 'unixepoch')
                            ELSE NEW.start_date
        END,
        next_due_date = CASE
                            WHEN typeof(NEW.next_due_date) = 'integer'
                                THEN date(NEW.next_due_date / 1000, 'unixepoch')
                            ELSE NEW.next_due_date
            END,
        date_added    = CASE
                            WHEN typeof(NEW.date_added) = 'integer' THEN datetime(NEW.date_added / 1000, 'unixepoch')
                            ELSE NEW.date_added
            END,
        last_updated  = CASE
                            WHEN typeof(NEW.last_updated) = 'integer'
                                THEN datetime(NEW.last_updated / 1000, 'unixepoch')
                            ELSE NEW.last_updated
            END
    WHERE id = NEW.id;

    UPDATE t_history
    SET description = NEW.description
    WHERE t_history.recursive_expense_fk = NEW.id
      AND t_history.description != NEW.description;

END;

DROP TRIGGER IF EXISTS after_update_on_t_recursive_expense_when_recursive_total_not_null;
CREATE TRIGGER after_update_on_t_recursive_expense_when_recursive_total_not_null
    AFTER UPDATE
    ON t_recursive_expense
    FOR EACH ROW
    WHEN NEW.recursive_total IS NOT NULL
BEGIN

    UPDATE t_recursive_expense
    SET is_active = CASE
                        WHEN NEW.recursive_total > NEW.recursive_count THEN TRUE
                        ELSE FALSE END
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
       tco.hexadecimal_color_code,
       tmp.name AS mode_payment,
       h.value,
       tcu.symbol,
       h.date,
       tp.name  AS place,
       h.is_pointed,
       bt.main_reason,
       h.date_added

FROM t_history h
         LEFT JOIN t_account ta
                   ON h.account_fk = ta.id
         LEFT JOIN t_category_type tct
                   ON h.category_type_fk = tct.id
         LEFT JOIN t_color tco
                   ON tct.color_fk = tco.id
         LEFT JOIN t_mode_payment tmp
                   ON h.mode_payment_fk = tmp.id
         LEFT JOIN t_currency tcu
                   ON ta.currency_fk = tcu.id
         LEFT JOIN t_place tp
                   ON h.place_fk = tp.id
         LEFT JOIN t_bank_transfer bt
                   ON h.bank_transfer_fk = bt.id;

DROP VIEW IF EXISTS v_category;
CREATE VIEW v_category AS
SELECT tct.id,
       tct.name AS category_name,
       tct.color_fk,
       tct.date_added AS date_category_added,
       tc.name AS color_name,
       tc.hexadecimal_color_code,
       tc.date_added AS date_color_added
FROM main.t_category_type tct
         INNER JOIN t_color tc
                    ON tct.color_fk = tc.id;

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
       ROUND(SUM(th.value), 2)                                              AS total,
       ROUND(SUM(CASE WHEN th.is_pointed = TRUE THEN th.value ELSE 0 END), 2)  AS total_pointed,
       ROUND(SUM(CASE WHEN th.is_pointed = FALSE THEN th.value ELSE 0 END), 2) AS total_not_pointed,
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

DROP VIEW IF EXISTS v_recursive_expense;
CREATE VIEW v_recursive_expense AS
SELECT tre.id,
       tre.account_fk,
       ta.name  AS account,
       tre.description,
       tre.note,
       tre.category_type_fk,
       tct.name AS category,
       tco.hexadecimal_color_code,
       tre.mode_payment_fk,
       tmp.name AS mode_payment,
       tre.value,
       tcr.symbol,
       tre.place_fk,
       tp.name  AS place,
       tre.start_date,
       tre.recursive_total,
       tre.recursive_count,
       tre.frequency_fk,
       trf.frequency,
       tre.next_due_date,
       tre.is_active,
       tre.force_deactivate,
       tre.date_added,
       tre.last_updated
FROM t_recursive_expense tre
         INNER JOIN t_account ta
                    ON tre.account_fk = ta.id
         INNER JOIN t_currency tcr
                    ON ta.currency_fk = tcr.id
         INNER JOIN t_category_type tct
                    ON tre.category_type_fk = tct.id
         INNER JOIN t_color tco
                    ON tct.color_fk = tco.id
         INNER JOIN t_mode_payment tmp
                    ON tre.mode_payment_fk = tmp.id
         INNER JOIN t_place tp
                    ON tre.place_fk = tp.id
         INNER JOIN t_recursive_frequency trf
                    ON tre.frequency_fk = trf.id;

-- region Analysis
DROP VIEW IF EXISTS analysis_v_account_monthly_cumulative_sum;
CREATE VIEW analysis_v_account_monthly_cumulative_sum AS
WITH all_periods AS (SELECT a.id                     AS account_fk,
                            a.name                   AS account,
                            tc.id                    AS currency_fk,
                            tc.symbol                AS currency,
                            y.year || '-' || m.month AS period
                     FROM t_account a
                              LEFT JOIN t_currency tc on a.currency_fk = tc.id
                              CROSS JOIN (
                                SELECT DISTINCT year
                                          FROM (
                                              SELECT strftime('%Y', h.date) AS year
                                              FROM t_history h
                                              UNION
                                              SELECT STRftime('%Y', CURRENT_DATE)
                                               )) AS y
                              CROSS JOIN (SELECT strftime('%m', date('2000-' || x || '-01')) AS month
                                          FROM (SELECT '01' AS x
                                                UNION
                                                SELECT '02'
                                                UNION
                                                SELECT '03'
                                                UNION
                                                SELECT '04'
                                                UNION
                                                SELECT '05'
                                                UNION
                                                SELECT '06'
                                                UNION
                                                SELECT '07'
                                                UNION
                                                SELECT '08'
                                                UNION
                                                SELECT '09'
                                                UNION
                                                SELECT '10'
                                                UNION
                                                SELECT '11'
                                                UNION
                                                SELECT '12')) m
                     WHERE (y.year || '-' || m.month) <= strftime('%Y-%m', CURRENT_DATE)),
     monthly AS (SELECT ap.account_fk,
                        ap.account,
                        ap.currency_fk,
                        ap.currency,
                        ap.period,
                        COALESCE(SUM(h.value), 0) as monthly_value
                 FROM all_periods ap
                          LEFT JOIN t_history h
                                    ON h.account_fk = ap.account_fk AND ap.period = strftime('%Y-%m', h.date)
                 GROUP BY ap.account_fk, ap.period),
     ranked AS (SELECT *,
                       ROW_NUMBER() OVER (PARTITION BY account_fk ORDER BY period) as rn
                FROM monthly),
     cumulative AS (SELECT r1.rn,
                           r1.period,
                           r1.account_fk,
                           r1.account,
                           r1.currency_fk,
                           r1.currency,
                           (SELECT SUM(r2.monthly_value)
                            FROM ranked r2
                            WHERE r2.rn <= r1.rn
                              AND r2.account_fk = r1.account_fk) as cumulative_sum
                    FROM ranked r1)
SELECT account_fk,
       account,
       period,
       ROUND(cumulative_sum, 2) as cumulative_sum,
       currency_fk,
       currency
FROM cumulative
ORDER BY account_fk, rn;

DROP VIEW IF EXISTS analysis_v_account_category_monthly_sum_positive_negative;
CREATE VIEW analysis_v_account_category_monthly_sum_positive_negative AS
WITH all_periods AS (SELECT a.id                      AS account_fk,
                            a.name                    AS account,
                            a.currency_fk             AS currency_fk,
                            tca.symbol                AS currency,
                            tct.id                    AS category_type_fk,
                            tct.name                  AS category_type,
                            tc.hexadecimal_color_code AS color_code,
                            y.year || '-' || m.month  AS period
                     FROM t_account a
                              LEFT JOIN t_currency tca ON a.currency_fk = tca.id
                              CROSS JOIN t_category_type tct
                              LEFT JOIN t_color tc ON tct.color_fk = tc.id
                              CROSS JOIN (
                                SELECT DISTINCT year
                                          FROM (
                                              SELECT strftime('%Y', h.date) AS year
                                              FROM t_history h
                                              UNION
                                              SELECT STRftime('%Y', CURRENT_DATE)
                                               )) AS y
                              CROSS JOIN (SELECT strftime('%m', date('2000-' || x || '-01')) AS month
                                          FROM (SELECT '01' AS x
                                                UNION
                                                SELECT '02'
                                                UNION
                                                SELECT '03'
                                                UNION
                                                SELECT '04'
                                                UNION
                                                SELECT '05'
                                                UNION
                                                SELECT '06'
                                                UNION
                                                SELECT '07'
                                                UNION
                                                SELECT '08'
                                                UNION
                                                SELECT '09'
                                                UNION
                                                SELECT '10'
                                                UNION
                                                SELECT '11'
                                                UNION
                                                SELECT '12')) m
                     WHERE (y.year || '-' || m.month) <= strftime('%Y-%m', CURRENT_DATE)),
     monthly AS (SELECT ap.account_fk,
                        ap.account,
                        ap.currency_fk,
                        ap.currency,
                        ap.category_type_fk,
                        ap.category_type,
                        ap.color_code,
                        ap.period,
                        COALESCE(SUM(CASE WHEN h.value < 0 THEN h.value ELSE 0 END), 0)  AS monthly_negative_value,
                        COALESCE(SUM(CASE WHEN h.value >= 0 THEN h.value ELSE 0 END), 0) AS monthly_positive_value
                 FROM all_periods ap
                          LEFT JOIN t_history h
                                    ON h.account_fk = ap.account_fk
                                        AND h.category_type_fk = ap.category_type_fk
                                        AND ap.period = strftime('%Y-%m', h.date)
                 GROUP BY ap.account_fk, ap.category_type_fk, ap.period)
SELECT account_fk,
       account,
       category_type,
       color_code,
       period,
       ROUND(monthly_negative_value, 2) AS monthly_negative_sum,
       ROUND(monthly_positive_value, 2) AS monthly_positive_sum,
       currency_fk,
       currency
FROM monthly
ORDER BY account_fk, period, category_type;

DROP VIEW IF EXISTS analysis_v_account_category_monthly_sum;
CREATE VIEW analysis_v_account_category_monthly_sum AS
SELECT account_fk,
       account,
       category_type,
       color_code,
       period,
       ROUND(monthly_negative_sum + monthly_positive_sum, 2) AS monthly_sum,
       currency_fk,
       currency
FROM analysis_v_account_category_monthly_sum_positive_negative
ORDER BY account_fk, period, category_type;

DROP VIEW IF EXISTS analysis_v_account_mode_payment_category_monthly_sum;
CREATE VIEW analysis_v_account_mode_payment_category_monthly_sum AS
WITH all_periods AS (SELECT a.id                     AS account_fk,
                            a.name                   AS account,
                            tcu.id                   AS currency_fk,
                            tcu.symbol               AS currency,
                            tmp.id                   AS mode_payment_fk,
                            tmp.name                 AS mode_payment,
                            y.year || '-' || m.month AS period,
                            ct.id                    AS category_fk,
                            ct.name                  AS category,
                            tc.hexadecimal_color_code
                     FROM t_account a
                              CROSS JOIN t_mode_payment tmp
                              CROSS JOIN t_category_type ct
                              LEFT JOIN t_color tc ON ct.color_fk = tc.id
                              LEFT JOIN t_currency tcu on a.currency_fk = tcu.id
                              CROSS JOIN (
                                SELECT DISTINCT year
                                          FROM (
                                              SELECT strftime('%Y', h.date) AS year
                                              FROM t_history h
                                              UNION
                                              SELECT STRftime('%Y', CURRENT_DATE)
                                               )) AS y
                              CROSS JOIN (SELECT strftime('%m', date('2000-' || x || '-01')) AS month
                                          FROM (SELECT '01' AS x
                                                UNION
                                                SELECT '02'
                                                UNION
                                                SELECT '03'
                                                UNION
                                                SELECT '04'
                                                UNION
                                                SELECT '05'
                                                UNION
                                                SELECT '06'
                                                UNION
                                                SELECT '07'
                                                UNION
                                                SELECT '08'
                                                UNION
                                                SELECT '09'
                                                UNION
                                                SELECT '10'
                                                UNION
                                                SELECT '11'
                                                UNION
                                                SELECT '12')) m
                     WHERE (y.year || '-' || m.month) <= strftime('%Y-%m', CURRENT_DATE)),
     monthly AS (SELECT ap.account_fk,
                        ap.account,
                        ap.currency_fk,
                        ap.currency,
                        ap.mode_payment_fk,
                        ap.mode_payment,
                        ap.period,
                        ap.category_fk,
                        ap.category,
                        ap.hexadecimal_color_code,
                        COUNT(CASE WHEN h.value IS NOT NULL THEN h.mode_payment_fk END) AS monthly_mode_payment,
                        COALESCE(SUM(h.value), 0)                                       AS monthly_value
                 FROM all_periods ap
                          LEFT JOIN t_history h
                                    ON h.account_fk = ap.account_fk
                                        AND h.mode_payment_fk = ap.mode_payment_fk
                                        AND h.category_type_fk = ap.category_fk
                                        AND ap.period = strftime('%Y-%m', h.date)
                 GROUP BY ap.account_fk, ap.mode_payment_fk, ap.period, ap.category_fk)
SELECT account_fk,
       account,
       mode_payment,
       period,
       category,
       hexadecimal_color_code,
       ROUND(monthly_value, 2) AS monthly_sum,
       currency_fk,
       currency,
       monthly_mode_payment
FROM monthly
ORDER BY account_fk, period, mode_payment, category;

DROP VIEW IF EXISTS analysis_v_budget_monthly;
CREATE VIEW analysis_v_budget_monthly AS
WITH date_bounds AS (SELECT STRFTIME('%Y-%m', MIN(h.date)) AS min_date
                     FROM t_history h),

     months AS (SELECT STRFTIME('%Y-%m', DATE('now', 'start of month')) AS period
                UNION ALL
                SELECT STRFTIME('%Y-%m', DATE(period || '-01', '-1 month'))
                FROM months
                WHERE DATE(period || '-01') > (SELECT DATE(min_date || '-01') FROM date_bounds)),

     filtered_accounts AS (SELECT DISTINCT a.id AS account_fk, a.name AS account_name, tc.id AS symbol_fk, tc.symbol
                           FROM t_account a
                                    INNER JOIN t_history h ON h.account_fk = a.id
                                    INNER JOIN t_currency tc ON a.currency_fk = tc.id),

     monthly_values AS (SELECT a.id                      AS account_fk,
                               a.name                    AS account_name,
                               tc.id                     AS symbol_fk,
                               tc.symbol                 AS symbol,
                               STRFTIME('%Y-%m', h.date) AS period,
                               SUM(h.value)              AS total_value
                        FROM t_history h
                                 INNER JOIN t_account a ON h.account_fk = a.id
                                 INNER JOIN t_currency tc ON a.currency_fk = tc.id
                        GROUP BY a.id, period),

     account_months AS (SELECT a.account_fk, a.account_name, a.symbol_fk, a.symbol, m.period
                        FROM filtered_accounts a
                                 CROSS JOIN months m),

     cumulative_values AS (SELECT am.account_fk,
                                  am.account_name,
                                  am.symbol_fk,
                                  am.symbol,
                                  am.period,
                                  COALESCE(
                                          (SELECT SUM(mv2.total_value)
                                           FROM monthly_values mv2
                                           WHERE mv2.account_fk = am.account_fk
                                             AND mv2.period <= am.period),
                                          0
                                  ) AS cumulative_total_value
                           FROM account_months am
                                    LEFT JOIN monthly_values mv
                                              ON am.account_fk = mv.account_fk AND am.period = mv.period)

SELECT cv.account_fk,
       cv.account_name,
       cv.symbol_fk,
       cv.symbol,
       cv.period,
       ROUND(cv.cumulative_total_value, 2)                                                AS period_value,
       STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 month'))                            AS previous_period,
       ROUND(COALESCE(pre_cv.cumulative_total_value, 0), 2)                               AS previous_period_value,
       CASE
           WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Gain'
           WHEN cv.cumulative_total_value < COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Deficit'
           ELSE 'Stable'
           END                                                                            AS status,
       ROUND(
               CASE
                   WHEN cv.cumulative_total_value = COALESCE(pre_cv.cumulative_total_value, 0) THEN 0
                   ELSE 100 * CASE
                                  WHEN COALESCE(pre_cv.cumulative_total_value, 0) = 0 THEN
                                      CASE
                                          WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0)
                                              THEN 1
                                          ELSE -1
                                          END
                                  ELSE (cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0))
                                      / ABS(COALESCE(pre_cv.cumulative_total_value, 1))
                       END
                   END, 2
       )                                                                                  AS percentage,
       ROUND((cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0)), 2) AS difference_value
FROM cumulative_values cv
         LEFT JOIN cumulative_values pre_cv
                   ON cv.account_fk = pre_cv.account_fk
                       AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 month')) = pre_cv.period
ORDER BY cv.account_fk, cv.period;

DROP VIEW IF EXISTS analysis_v_budget_monthly_global;
CREATE VIEW analysis_v_budget_monthly_global AS
WITH date_bounds AS (SELECT STRFTIME('%Y-%m', MIN(h.date)) AS min_date
                     FROM t_history h),
     months AS (SELECT STRFTIME('%Y-%m', DATE('now', 'start of month')) AS period
                UNION ALL
                SELECT STRFTIME('%Y-%m', DATE(period || '-01', '-1 month'))
                FROM months
                WHERE DATE(period || '-01') > (SELECT DATE(min_date || '-01') FROM date_bounds)),
     monthly_values AS (SELECT STRFTIME('%Y-%m', h.date) AS period,
                               SUM(h.value)              AS total_value
                        FROM t_history h
                        GROUP BY period),
     cumulative_values AS (SELECT m.period,
                                  COALESCE(
                                          (SELECT SUM(mv2.total_value)
                                           FROM monthly_values mv2
                                           WHERE mv2.period <= m.period),
                                          0
                                  ) AS cumulative_total_value
                           FROM months m)
SELECT cv.period,
       ROUND(cv.cumulative_total_value, 2)                                                AS period_value,
       STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 month'))                            AS previous_period,
       ROUND(COALESCE(pre_cv.cumulative_total_value, 0), 2)                               AS previous_period_value,
       CASE
           WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Gain'
           WHEN cv.cumulative_total_value < COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Deficit'
           ELSE 'Stable'
           END                                                                            AS status,
       ROUND(
               CASE
                   WHEN cv.cumulative_total_value = COALESCE(pre_cv.cumulative_total_value, 0) THEN 0
                   ELSE 100 * CASE
                                  WHEN COALESCE(pre_cv.cumulative_total_value, 0) = 0 THEN
                                      CASE
                                          WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0)
                                              THEN 1
                                          ELSE -1
                                          END
                                  ELSE (cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0))
                                      / ABS(COALESCE(pre_cv.cumulative_total_value, 1))
                       END
                   END, 2
       )                                                                                  AS percentage,
       ROUND((cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0)), 2) AS difference_value
FROM cumulative_values cv
         LEFT JOIN cumulative_values pre_cv
                   ON STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 month')) = pre_cv.period
ORDER BY cv.period;

DROP VIEW IF EXISTS analysis_v_budget_period_annual;
CREATE VIEW analysis_v_budget_period_annual AS
WITH date_bounds AS (SELECT STRFTIME('%Y-%m', MIN(h.date)) AS min_date
                     FROM t_history h),

     months AS (SELECT STRFTIME('%Y-%m', DATE('now', 'start of month')) AS period
                UNION ALL
                SELECT STRFTIME('%Y-%m', DATE(period || '-01', '-1 month'))
                FROM months
                WHERE DATE(period || '-01') > (SELECT DATE(min_date || '-01') FROM date_bounds)),

     filtered_accounts AS (SELECT DISTINCT a.id   AS account_fk,
                                           a.name AS account_name,
                                           tc.id  AS symbol_fk,
                                           tc.symbol
                           FROM t_account a
                                    INNER JOIN t_history h
                                               ON h.account_fk = a.id
                                    INNER JOIN t_currency tc
                                               ON a.currency_fk = tc.id),

     account_months AS (SELECT a.account_fk,
                               a.account_name,
                               a.symbol_fk,
                               a.symbol,
                               m.period
                        FROM filtered_accounts a
                                 CROSS JOIN months m),

     monthly_values AS (SELECT a.id                      AS account_fk,
                               a.name                    AS account_name,
                               tc.id                     AS symbol_fk,
                               tc.symbol                 AS symbol,
                               STRFTIME('%Y-%m', h.date) AS period,
                               SUM(h.value)              AS total_value
                        FROM t_history h
                                 INNER JOIN t_account a
                                            ON h.account_fk = a.id
                                 INNER JOIN t_currency tc
                                            ON a.currency_fk = tc.id
                        GROUP BY a.id, period),

     cumulative_values AS (SELECT am.account_fk,
                                  am.account_name,
                                  am.symbol_fk,
                                  am.symbol,
                                  am.period,
                                  STRFTIME('%m', am.period)                   AS month_of_year,
                                  STRFTIME('%Y', am.period)                   AS year,
                                  COALESCE(
                                          (SELECT SUM(mv2.total_value)
                                           FROM monthly_values mv2
                                           WHERE mv2.account_fk = am.account_fk
                                             AND mv2.period <= am.period), 0) AS cumulative_total_value
                           FROM account_months am
                                    LEFT JOIN monthly_values mv
                                              ON am.account_fk = mv.account_fk AND am.period = mv.period)

SELECT cv.account_fk,
       cv.account_name,
       cv.symbol_fk,
       cv.symbol,
       cv.period,
       ROUND(cv.cumulative_total_value, 2)                              AS period_value,
       COALESCE(
               STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')),
               CAST(cv.year AS INTEGER) - 1 || '-' || cv.month_of_year) AS previous_period,
       COALESCE(
               (SELECT ROUND(pre_cv.cumulative_total_value, 2)
                FROM cumulative_values pre_cv
                WHERE pre_cv.account_fk = cv.account_fk
                  AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) = pre_cv.period),
               0)                                                       AS previous_period_value,
       CASE
           WHEN cv.cumulative_total_value > COALESCE(
                   (SELECT pre_cv.cumulative_total_value
                    FROM cumulative_values pre_cv
                    WHERE pre_cv.account_fk = cv.account_fk
                      AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) = pre_cv.period),
                   0) THEN 'Gain'
           WHEN cv.cumulative_total_value < COALESCE(
                   (SELECT pre_cv.cumulative_total_value
                    FROM cumulative_values pre_cv
                    WHERE pre_cv.account_fk = cv.account_fk
                      AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) = pre_cv.period),
                   0) THEN 'Deficit'
           ELSE 'Stable'
           END                                                          AS status,
       ROUND(
               CASE
                   WHEN cv.cumulative_total_value = COALESCE(
                           (SELECT pre_cv.cumulative_total_value
                            FROM cumulative_values pre_cv
                            WHERE pre_cv.account_fk = cv.account_fk
                              AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) =
                                  pre_cv.period), 0) THEN 0
                   ELSE 100 * CASE
                                  WHEN COALESCE(
                                               (SELECT pre_cv.cumulative_total_value
                                                FROM cumulative_values pre_cv
                                                WHERE pre_cv.account_fk = cv.account_fk
                                                  AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) =
                                                      pre_cv.period), 0) = 0 THEN
                                      CASE
                                          WHEN cv.cumulative_total_value > COALESCE(
                                                  (SELECT pre_cv.cumulative_total_value
                                                   FROM cumulative_values pre_cv
                                                   WHERE pre_cv.account_fk = cv.account_fk
                                                     AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) =
                                                         pre_cv.period), 0) THEN 1
                                          ELSE -1
                                          END
                                  ELSE (cv.cumulative_total_value - COALESCE(
                                          (SELECT pre_cv.cumulative_total_value
                                           FROM cumulative_values pre_cv
                                           WHERE pre_cv.account_fk = cv.account_fk
                                             AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) =
                                                 pre_cv.period), 0)) / ABS(
                                               COALESCE(
                                                       (SELECT pre_cv.cumulative_total_value
                                                        FROM cumulative_values pre_cv
                                                        WHERE pre_cv.account_fk = cv.account_fk
                                                          AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) =
                                                              pre_cv.period), 1))
                       END
                   END, 2)                                              AS percentage,
       ROUND(
               (cv.cumulative_total_value - COALESCE(
                       (SELECT pre_cv.cumulative_total_value
                        FROM cumulative_values pre_cv
                        WHERE pre_cv.account_fk = cv.account_fk
                          AND STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) =
                              pre_cv.period), 0)), 2)                   AS difference_value
FROM cumulative_values cv
ORDER BY account_fk, period;

DROP VIEW IF EXISTS analysis_v_budget_period_annual_global;
CREATE VIEW analysis_v_budget_period_annual_global AS
WITH date_bounds AS (SELECT STRFTIME('%Y-%m', MIN(h.date)) AS min_date
                     FROM t_history h),
     months AS (SELECT STRFTIME('%Y-%m', DATE('now', 'start of month')) AS period
                UNION ALL
                SELECT STRFTIME('%Y-%m', DATE(period || '-01', '-1 month'))
                FROM months
                WHERE DATE(period || '-01') > (SELECT DATE(min_date || '-01') FROM date_bounds)),
     monthly_values AS (SELECT STRFTIME('%Y-%m', h.date) AS period,
                               SUM(h.value)              AS total_value
                        FROM t_history h
                        GROUP BY period),
     complete_monthly_values AS (SELECT m.period,
                                        COALESCE(mv.total_value, 0) AS total_value
                                 FROM months m
                                          LEFT JOIN monthly_values mv ON m.period = mv.period),
     cumulative_values AS (SELECT cmv.period,
                                  STRFTIME('%m', cmv.period)        AS month_of_year,
                                  STRFTIME('%Y', cmv.period)        AS year,
                                  (SELECT SUM(cmv2.total_value)
                                   FROM complete_monthly_values cmv2
                                   WHERE cmv2.period <= cmv.period) AS cumulative_total_value
                           FROM complete_monthly_values cmv)
SELECT cv.period,
       ROUND(cv.cumulative_total_value, 2)                                                as period_value,
       STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year'))                             as previous_period,
       COALESCE(ROUND(pre_cv.cumulative_total_value, 2), 0)                               as previous_period_value,
       CASE
           WHEN cv.cumulative_total_value >= COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Gain'
           ELSE 'Deficit'
           END                                                                            as status,
       ROUND(
               CASE
                   WHEN cv.cumulative_total_value = COALESCE(pre_cv.cumulative_total_value, 0) THEN 0
                   ELSE 100 * CASE
                                  WHEN COALESCE(pre_cv.cumulative_total_value, 0) = 0 THEN
                                      CASE
                                          WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0)
                                              THEN 1
                                          ELSE -1
                                          END
                                  ELSE (cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0))
                                      / ABS(COALESCE(pre_cv.cumulative_total_value, 1))
                       END
                   END, 2
       )                                                                                  AS percentage,
       ROUND((cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0)), 2) as difference_value
FROM cumulative_values cv
         LEFT JOIN cumulative_values pre_cv
                   ON STRFTIME('%Y-%m', DATE(cv.period || '-01', '-1 year')) = pre_cv.period
ORDER BY cv.period;

DROP VIEW IF EXISTS analysis_v_budget_total_annual;
CREATE VIEW analysis_v_budget_total_annual AS
WITH date_bounds AS (SELECT CAST(STRFTIME('%Y', MIN(h.date)) AS INTEGER) AS min_year,
                            CAST(STRFTIME('%Y', 'now') AS INTEGER)       AS max_year
                     FROM t_history h),
     years AS (SELECT (SELECT min_year FROM date_bounds) AS year
               UNION ALL
               SELECT CAST(year AS INTEGER) + 1
               FROM years,
                    date_bounds
               WHERE CAST(year AS INTEGER) + 1 <= (SELECT max_year FROM date_bounds)),
     account_years AS (SELECT a.id   as account_fk,
                              a.name as account_name,
                              yc.year
                       FROM t_account a
                                CROSS JOIN years yc),
     annual_values AS (SELECT ay.account_fk,
                              ay.account_name,
                              tc.id                     AS symbol_fk,
                              tc.symbol                 AS symbol,
                              ay.year,
                              COALESCE(SUM(h.value), 0) AS total_value
                       FROM account_years ay
                                LEFT JOIN t_history h
                                          ON ay.account_fk = h.account_fk AND STRFTIME('%Y', h.date) = CAST(ay.year AS TEXT)
                                INNER JOIN t_account a
                                           ON ay.account_fk = a.id
                                INNER JOIN t_currency tc
                                           ON tc.id = a.currency_fk
                       GROUP BY ay.account_fk, ay.year
                       ORDER BY ay.account_fk, ay.year),
     cumulative_values AS (SELECT av.account_fk,
                                  av.account_name,
                                  av.symbol_fk,
                                  av.symbol,
                                  av.year,
                                  (SELECT SUM(av2.total_value)
                                   FROM annual_values av2
                                   WHERE av2.account_fk = av.account_fk
                                     AND av2.year <= av.year) AS cumulative_total_value
                           FROM annual_values av)
SELECT cv.account_fk,
       cv.account_name,
       cv.symbol_fk,
       cv.symbol,
       CAST(cv.year AS INTEGER)                                                           AS period,
       ROUND(cv.cumulative_total_value, 2)                                                AS period_value,
       CAST(cv.year AS INTEGER) - 1                                                       AS previous_period,
       COALESCE(ROUND(pre_cv.cumulative_total_value, 2), 0)                               AS previous_period_value,
       CASE
           WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Gain'
           WHEN cv.cumulative_total_value < COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Deficit'
           ELSE 'Stable'
           END                                                                            AS status,
       ROUND(
               CASE
                   WHEN cv.cumulative_total_value = COALESCE(pre_cv.cumulative_total_value, 0) THEN 0
                   WHEN COALESCE(pre_cv.cumulative_total_value, 0) = 0 THEN
                       CASE
                           WHEN cv.cumulative_total_value > 0 THEN 100
                           ELSE -100
                           END
                   ELSE 100 * (cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0)) /
                        ABS(COALESCE(pre_cv.cumulative_total_value, 1))
                   END, 2)                                                                AS percentage,
       ROUND((cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0)), 2) AS difference_value
FROM cumulative_values cv
         LEFT JOIN cumulative_values pre_cv
                   ON cv.account_fk = pre_cv.account_fk
                       AND CAST(cv.year AS INTEGER) - 1 = CAST(pre_cv.year AS INTEGER);

DROP VIEW IF EXISTS analysis_v_budget_total_annual_global;
CREATE VIEW analysis_v_budget_total_annual_global AS
WITH date_bounds AS (SELECT CAST(STRFTIME('%Y', MIN(h.date)) AS INTEGER) AS min_year,
                            CAST(STRFTIME('%Y', 'now') AS INTEGER)       AS max_year
                     FROM t_history h),
     years AS (SELECT (SELECT min_year FROM date_bounds) AS year
               UNION ALL
               SELECT year + 1
               FROM years,
                    date_bounds
               WHERE year + 1 <= (SELECT max_year FROM date_bounds)),
     annual_values AS (SELECT yc.year,
                              COALESCE(SUM(h.value), 0) AS total_value
                       FROM years yc
                                LEFT JOIN t_history h
                                          ON STRFTIME('%Y', h.date) = CAST(yc.year AS TEXT)
                       GROUP BY yc.year),
     cumulative_values AS (SELECT av.year,
                                  (SELECT SUM(av2.total_value)
                                   FROM annual_values av2
                                   WHERE av2.year <= av.year) as cumulative_total_value
                           FROM annual_values av)
SELECT cv.year                                                                            AS period,
       ROUND(cv.cumulative_total_value, 2)                                                as period_value,
       cv.year - 1                                                                        as previous_period,
       COALESCE(ROUND(pre_cv.cumulative_total_value, 2), 0)                               as previous_period_value,
       CASE
           WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Gain'
           WHEN cv.cumulative_total_value < COALESCE(pre_cv.cumulative_total_value, 0) THEN 'Deficit'
           ELSE 'Stable'
           END                                                                            as status,
       ROUND(
               CASE
                   WHEN cv.cumulative_total_value = COALESCE(pre_cv.cumulative_total_value, 0) THEN 0
                   ELSE 100 * CASE
                                  WHEN COALESCE(pre_cv.cumulative_total_value, 0) = 0 THEN
                                      CASE
                                          WHEN cv.cumulative_total_value > COALESCE(pre_cv.cumulative_total_value, 0)
                                              THEN 1
                                          ELSE -1
                                          END
                                  ELSE (cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0))
                                      / ABS(COALESCE(pre_cv.cumulative_total_value, 1))
                       END
                   END, 2
       )                                                                                  AS percentage,
       ROUND((cv.cumulative_total_value - COALESCE(pre_cv.cumulative_total_value, 0)), 2) as difference_value
FROM cumulative_values cv
         LEFT JOIN cumulative_values pre_cv
                   ON cv.year - 1 = pre_cv.year;

DROP VIEW IF EXISTS v_bank_transfer_summary;
CREATE VIEW v_bank_transfer_summary AS
WITH from_account_balance_before AS (SELECT bt.id,
                                            SUM(th.value) AS balance
                                     FROM t_bank_transfer bt
                                              LEFT JOIN t_history th
                                                        ON th.account_fk = bt.from_account_fk AND th.date < bt.date
                                     GROUP BY bt.id),
     to_account_balance_before AS (SELECT bt.id,
                                          SUM(th.value) AS balance
                                   FROM t_bank_transfer bt
                                            LEFT JOIN t_history th
                                                      ON th.account_fk = bt.to_account_fk AND th.date < bt.date
                                   GROUP BY bt.id)
SELECT bt.id,
       a1.name                          AS from_account_name,
       c1.symbol                        AS from_account_symbol,
       a2.name                          AS to_account_name,
       c2.symbol                        AS to_account_symbol,
       bt.main_reason,
       bt.additional_reason,
       tct.name                         AS category_name,
       c.hexadecimal_color_code         AS category_color,
       tmp.name                         AS mode_payment,
       bt.date,
       ROUND(fab.balance, 2)            AS from_account_balance_before,
       ROUND(fab.balance - bt.value, 2) AS from_account_balance_after,
       ROUND(bt.value, 2)               AS value,
       ROUND(tab.balance, 2)            AS to_account_balance_before,
       ROUND(tab.balance + bt.value, 2) AS to_account_balance_after,
       bt.date_added
FROM t_bank_transfer bt
         INNER JOIN t_account a1
                    ON bt.from_account_fk = a1.id
         INNER JOIN t_account a2
                    ON bt.to_account_fk = a2.id
         INNER JOIN t_currency c1
                    ON a1.currency_fk = c1.id
         INNER JOIN t_currency c2
                    ON a2.currency_fk = c2.id
         INNER JOIN from_account_balance_before fab
                    ON fab.id = bt.id
         INNER JOIN to_account_balance_before tab
                    ON tab.id = bt.id
         INNER JOIN t_history h1
                    ON a1.id = h1.account_fk AND bt.id = h1.bank_transfer_fk
         INNER JOIN t_category_type tct
                    ON h1.category_type_fk = tct.id
         INNER JOIN t_color c
                    ON tct.color_fk = c.id
         INNER JOIN t_mode_payment tmp
                    ON h1.mode_payment_fk = tmp.id;

-- endregion

-- region Exports
DROP VIEW IF EXISTS export_v_recursive_frequency;
CREATE VIEW export_v_recursive_frequency AS
SELECT trf.id,
       trf.frequency,
       trf.description
FROM t_recursive_frequency trf;

DROP VIEW IF EXISTS export_v_account_type;
CREATE VIEW export_v_account_type AS
SELECT tat.id,
       tat.name,
       tat.date_added
FROM t_account_type tat;

DROP VIEW IF EXISTS export_v_currency;
CREATE VIEW export_v_currency AS
SELECT tc.id,
       tc.symbol,
       tc.date_added
FROM t_currency tc;

DROP VIEW IF EXISTS export_v_account;
CREATE VIEW export_v_account AS
SELECT ta.id,
       ta.name,
       tat.name  AS account_type,
       tc.symbol AS currency,
       ta.active,
       ta.date_added
FROM t_account ta
         INNER JOIN t_account_type tat
                    ON ta.account_type_fk = tat.id
         INNER JOIN t_currency tc
                    ON ta.currency_fk = tc.id;

DROP VIEW IF EXISTS export_v_color;
CREATE VIEW export_v_color AS
SELECT tc.id,
       tc.name,
       tc.hexadecimal_color_code,
       tc.date_added
FROM t_color tc;

DROP VIEW IF EXISTS export_v_category_type;
CREATE VIEW export_v_category_type AS
SELECT tct.id,
       tct.name,
       tc.name AS color_name,
       tct.date_added
FROM t_category_type tct
         INNER JOIN t_color tc
                    ON tct.color_fk = tc.id;

DROP VIEW IF EXISTS export_v_bank_transfer;
CREATE VIEW export_v_bank_transfer AS
SELECT tbt.id,
       tbt.value,
       ta_fr.name AS from_account_name,
       ta_to.name AS to_account_name,
       tbt.main_reason,
       tbt.additional_reason,
       tbt.date,
       tbt.date_added
FROM t_bank_transfer tbt
         INNER JOIN t_account ta_fr
                    ON ta_fr.id = tbt.from_account_fk
         INNER JOIN t_account ta_to
                    ON ta_to.id = tbt.to_account_fk;

DROP VIEW IF EXISTS export_v_mode_payment;
CREATE VIEW export_v_mode_payment AS
SELECT tmp.id,
       tmp.name,
       tmp.date_added
FROM t_mode_payment tmp;

DROP VIEW IF EXISTS export_v_place;
CREATE VIEW export_v_place AS
SELECT tp.id,
       tp.name,
       tp.number,
       tp.street,
       tp.postal,
       tp.city,
       tp.country,
       tp.latitude,
       tp.longitude,
       tp.is_open,
       tp.can_be_deleted,
       tp.date_added
FROM t_place tp;

DROP VIEW IF EXISTS export_v_recursive_expense;
CREATE VIEW export_v_recursive_expense AS
SELECT tre.id,
       ta.name       AS account_name,
       tre.description,
       tre.note,
       tct.name      AS category_type,
       tmp.name      AS mode_payment,
       tre.value,
       tp.name       AS place_name,
       tre.start_date,
       tre.recursive_total,
       tre.recursive_count,
       trf.frequency AS frequency,
       tre.next_due_date,
       tre.is_active,
       tre.force_deactivate,
       tre.date_added,
       tre.last_updated
FROM t_recursive_expense tre
         INNER JOIN t_account ta
                    ON tre.account_fk = ta.id
         INNER JOIN t_category_type tct
                    ON tre.category_type_fk = tct.id
         INNER JOIN t_mode_payment tmp
                    ON tre.mode_payment_fk = tmp.id
         INNER JOIN t_place tp
                    ON tre.place_fk = tp.id
         INNER JOIN t_recursive_frequency trf
                    ON tre.frequency_fk = trf.id;

DROP VIEW IF EXISTS export_v_history;
CREATE VIEW export_v_history AS
SELECT th.id,
       ta.name AS account_name,
       th.description,
       tct.name AS category_type,
       tmp.name AS mode_payment,
       th.value,
       th.date,
       tp.name AS place,
       th.is_pointed,
--        th.bank_transfer_fk,
--        th.recursive_expense_fk,
       th.date_added,
       th.date_pointed
FROM t_history th
    INNER JOIN t_account ta
        ON th.account_fk = ta.id
    INNER JOIN t_category_type tct
        ON th.category_type_fk = tct.id
    INNER JOIN t_mode_payment tmp
        ON th.mode_payment_fk = tmp.id
    INNER JOIN t_place tp
        ON th.place_fk = tp.id;

-- endregion

-- endregion
