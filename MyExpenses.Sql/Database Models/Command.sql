VACUUM;

SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1
  AND pointed = 1;

SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1
  AND pointed = 0;

SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1;

ALTER TABLE t_history
    RENAME pointed TO is_pointed;

DELETE
FROM t_supported_languages;
INSERT INTO t_supported_languages(id, code, native_name, english_name, default_language, date_added)
VALUES (1, 'en-001', 'English (World)', 'English (World)', true, '2024-07-12 09:19:06'),
       (2, 'fr-FR', 'Français (France)', 'French (France)', false, '2024-07-12 09:19:06');

SELECT load_extension('mod_spatialite');

SELECT 'POINT(' || longitude || ' ' || latitude || ')'
FROM t_place;

SELECT *
FROM v_history h
WHERE h.date BETWEEN '2024-08-01' AND '2024-09-01';

CREATE TABLE t_account_2 AS
SELECT * FROM t_account;

DROP TABLE IF EXISTS t_account;
create table t_account
(
    id              INTEGER
        constraint t_account_pk
            primary key autoincrement,
    name            TEXT(55),
    account_type_fk INTEGER
        constraint t_account_t_account_type_id_fk
            references t_account_type,
    currency_fk     INTEGER
        constraint t_account_t_currency_id_fk
            references t_currency,
    active          BOOLEAN  default TRUE,
    date_added      DATETIME default CURRENT_TIMESTAMP
);

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


INSERT INTO t_account
SELECT * FROM t_account_2;

DROP TABLE t_account_2;
