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

SELECT name, length(name) AS length
FROM t_mode_payment
ORDER BY length DESC;

CREATE TABLE t_mode_payment_2 AS
SELECT * FROM t_mode_payment;

DROP TABLE IF EXISTS t_mode_payment;
create table t_mode_payment
(
    id             INTEGER
        constraint t_mode_payment_pk
            primary key autoincrement,
    name           TEXT(55),
    can_be_deleted BOOLEAN  default 1,
    date_added     DATETIME default CURRENT_TIMESTAMP
);

DROP TRIGGER IF EXISTS after_insert_on_after_insert_on_t_mode_payment;
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

INSERT INTO t_mode_payment
SELECT * FROM t_mode_payment_2;

DROP TABLE t_mode_payment_2;
VACUUM;
