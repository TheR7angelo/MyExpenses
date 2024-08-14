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
    code             TEXT    NOT NULL
        CONSTRAINT t_supported_languages_pk_2
            UNIQUE,
    native_name      TEXT    NOT NULL,
    english_name     TEXT    NOT NULL,
    default_language BOOLEAN NOT NULL DEFAULT FALSE,
    date_added       DATETIME         DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_recursive_frequency;
CREATE TABLE t_recursive_frequency
(
    id          INTEGER
        CONSTRAINT t_recursive_frequency_pk
            PRIMARY KEY AUTOINCREMENT ,
    frequency   TEXT,
    description TEXT
);

DROP TABLE IF EXISTS t_spatial_ref_sys;
CREATE TABLE t_spatial_ref_sys
(
    srid      INTEGER not null
        constraint t_spatial_ref_sys_pk
            primary key,
    auth_name TEXT    not null,
    auth_srid TEXT    not null,
    srtext    TEXT    not null,
    proj4text TEXT    not null,
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
    id             INTEGER
        CONSTRAINT t_place_pk
            PRIMARY KEY AUTOINCREMENT,
    name           TEXT,
    number         TEXT,
    street         TEXT,
    postal         TEXT,
    city           TEXT,
    country        TEXT,
    latitude       REAL,
    longitude      REAL,
    geometry       BLOB,
    is_open        BOOLEAN  DEFAULT TRUE,
    can_be_deleted BOOLEAN  DEFAULT TRUE,
    date_added     DATETIME DEFAULT CURRENT_TIMESTAMP
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
    main_reason       TEXT,
    additional_reason TEXT,
    date              DATETIME,
    date_added        DATETIME default CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS t_recursive_expense;
CREATE TABLE t_recursive_expense
(
    id                INTEGER
        CONSTRAINT t_recursive_expense_pk
            PRIMARY KEY AUTOINCREMENT ,
    account_fk        INTEGER
        CONSTRAINT t_recursive_expense_t_account_id_fk
            REFERENCES t_account,
    description       TEXT,
    category_type_fk  INTEGER
        CONSTRAINT t_recursive_expense_t_category_type_id_fk
            REFERENCES t_category_type,
    mode_payment_fk   INTEGER
        CONSTRAINT t_recursive_expense_t_mode_payment_id_fk
            REFERENCES t_mode_payment,
    value             REAL,
    place_fk          INTEGER DEFAULT 0
        CONSTRAINT t_recursive_expense_t_place_id_fk
            REFERENCES t_place,
    start_date        DATE              NOT NULL,
    recursive_total   INTEGER,
    recursive_count INTEGER DEFAULT 0 NOT NULL,
    frequency_fk      INTEGER           NOT NULL
        CONSTRAINT t_recursive_expense_t_recursive_frequency_id_fk
            REFERENCES t_recursive_frequency,
    next_due_date     DATE              NOT NULL,
    is_active         BOOLEAN DEFAULT TRUE,
    date_added        DATETIME    DEFAULT CURRENT_TIMESTAMP
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
    recursive_expense_fk INTEGER
        CONSTRAINT t_history_t_recursive_expense_id_fk
            REFERENCES t_recursive_expense,
    date_added       DATETIME DEFAULT CURRENT_TIMESTAMP,
    date_pointed     DATETIME
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
                           WHEN NEW.pointed = 1 AND typeof(NEW.date) = 'integer'
                               THEN datetime(NEW.date / 1000, 'unixepoch')
                           WHEN NEW.pointed = 0 THEN NULL
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
                           WHEN NEW.pointed = 1 AND typeof(NEW.date) = 'integer'
                               THEN datetime(NEW.date / 1000, 'unixepoch')
                           WHEN NEW.pointed = 0 THEN NULL
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
    SET start_date = CASE
                         WHEN typeof(NEW.start_date) = 'integer' THEN datetime(NEW.start_date / 1000, 'unixepoch')
                         ELSE NEW.start_date
            END,
        next_due_date = CASE
                         WHEN typeof(NEW.next_due_date) = 'integer' THEN datetime(NEW.next_due_date / 1000, 'unixepoch')
                         ELSE NEW.next_due_date
            END,
        date_added = CASE
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
    SET start_date = CASE
                         WHEN typeof(NEW.start_date) = 'integer' THEN datetime(NEW.start_date / 1000, 'unixepoch')
                         ELSE NEW.start_date
            END,
        next_due_date = CASE
                         WHEN typeof(NEW.next_due_date) = 'integer' THEN datetime(NEW.next_due_date / 1000, 'unixepoch')
                         ELSE NEW.next_due_date
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
       tco.hexadecimal_color_code,
       tmp.name AS mode_payment,
       h.value,
       tcu.symbol,
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
       ROUND(SUM(CASE WHEN th.pointed = TRUE THEN th.value ELSE 1 END), 2) AS total_pointed,
       ROUND(SUM(CASE WHEN th.pointed = FALSE THEN th.value ELSE 0 END), 2) AS total_not_pointed,
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

DROP VIEW IF EXISTS v_recursive_expense;
CREATE VIEW v_recursive_expense AS
SELECT tre.id,
       tre.account_fk,
       ta.name AS account,
       tre.description,
       tre.category_type_fk,
       tct.name AS category,
       tre.mode_payment_fk,
       tmp.name AS mode_payment,
       tre.value,
       tre.place_fk,
       tp.name AS place,
       tre.start_date,
       tre.recursive_total,
       tre.recursive_count,
       tre.frequency_fk,
       trf.frequency,
       tre.next_due_date,
       tre.is_active,
       tre.date_added
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

DROP VIEW IF EXISTS v_account_monthly_cumulative_sum;
CREATE VIEW v_account_monthly_cumulative_sum AS
WITH all_periods AS (SELECT a.id                     AS account_fk,
                            a.name                   AS account,
                            y.year || '-' || m.month AS period
                     FROM t_account a
                              CROSS JOIN (SELECT DISTINCT STRFTIME('%Y', h.date) AS year
                                          FROM t_history h) y
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
                     WHERE y.year < (SELECT strftime('%Y', MAX(date)) FROM t_history)
                        OR (y.year == (SELECT strftime('%Y', MAX(date)) FROM t_history)
                         AND m.month <= (SELECT strftime('%m', MAX(date)) FROM t_history))),
     monthly AS (SELECT ap.account_fk,
                        ap.account,
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
                           (SELECT SUM(r2.monthly_value)
                            FROM ranked r2
                            WHERE r2.rn <= r1.rn
                              AND r2.account_fk = r1.account_fk) as cumulative_sum
                    FROM ranked r1)
SELECT account_fk,
       account,
       period,
       ROUND(cumulative_sum, 2) as cumulative_sum
FROM cumulative
ORDER BY account_fk, rn;

DROP VIEW IF EXISTS v_account_category_monthly_sum;
CREATE VIEW v_account_category_monthly_sum AS
WITH all_periods AS (
         SELECT a.id                     AS account_fk,
                a.name                   AS account,
                tct.id                   AS category_type_fk,
                tct.name                 AS category_type,
                tc.hexadecimal_color_code AS color_code,
                y.year || '-' || m.month AS period
         FROM t_account a
                  CROSS JOIN t_category_type tct
                  LEFT JOIN t_color tc ON tct.color_fk = tc.id
                  CROSS JOIN (SELECT DISTINCT strftime('%Y', h.date) AS year
                              FROM t_history h) y
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
         WHERE y.year < (SELECT strftime('%Y', MAX(date)) FROM t_history)
            OR (y.year == (SELECT strftime('%Y', MAX(date)) FROM t_history)
                AND m.month <= (SELECT strftime('%m', MAX(date)) FROM t_history))),
     monthly AS (
         SELECT ap.account_fk,
                ap.account,
                ap.category_type_fk,
                ap.category_type,
                ap.color_code,
                ap.period,
                COALESCE(SUM(h.value), 0) as monthly_value
         FROM all_periods ap
                  LEFT JOIN t_history h
                            ON h.account_fk = ap.account_fk AND h.category_type_fk = ap.category_type_fk AND ap.period = strftime('%Y-%m', h.date)
         GROUP BY ap.account_fk, ap.category_type_fk, ap.period)
SELECT account_fk,
       account,
       category_type,
       color_code,
       period,
       ROUND(monthly_value, 2) AS monthly_sum
FROM monthly
ORDER BY account_fk, period, category_type;

-- endregion
