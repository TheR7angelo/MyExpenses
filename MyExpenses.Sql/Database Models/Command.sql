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
    RENAME compte_fk TO account_fk;

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

CREATE TABLE t_place_2 AS
SELECT * FROM t_place;

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
    is_open        BOOLEAN NOT NULL DEFAULT TRUE,
    can_be_deleted BOOLEAN DEFAULT TRUE,
    date_added     DATETIME DEFAULT CURRENT_TIMESTAMP
);

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

INSERT INTO t_place
SELECT * FROM t_place_2;

DROP TABLE t_place_2;
