VACUUM;

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

SELECT english_name, length(english_name) AS length
FROM t_supported_languages
ORDER BY length DESC;

CREATE TABLE t_supported_languages_2 AS
SELECT * FROM t_supported_languages;

DROP TABLE IF EXISTS t_supported_languages;
create table t_supported_languages
(
    id               INTEGER
        constraint t_supported_languages_pk
            primary key autoincrement,
    code             TEXT(10)                   not null
        constraint t_supported_languages_pk_2
            unique,
    native_name      TEXT(55)                   not null,
    english_name     TEXT(55)                   not null,
    default_language BOOLEAN  default FALSE not null,
    date_added       DATETIME default CURRENT_TIMESTAMP
);

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

INSERT INTO t_supported_languages
SELECT * FROM t_supported_languages_2;

DROP TABLE t_supported_languages_2;
VACUUM;
