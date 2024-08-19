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

SELECT name, geometry, ST_SRID(geometry), ST_ASTEXT(geometry)
FROM t_place;

UPDATE t_place
-- SET geometry = GeomFromText('POINT(' || longitude || ' ' || latitude || ')', 4326)
SET geometry = GeomFromText('POINT(' || 0 || ' ' || 0 || ')', 4326)
WHERE id = 1;

SELECT 'POINT(' || longitude || ' ' || latitude || ')'
FROM t_place;

INSERT INTO t_geometry_columns(f_table_name, f_geometry_column, type, coord_dimension, srid)
VALUES ('t_place', 'geometry', 'POINT', 2, 4326);


SELECT *
FROM v_history h
WHERE h.date BETWEEN '2024-08-01' AND '2024-09-01';
