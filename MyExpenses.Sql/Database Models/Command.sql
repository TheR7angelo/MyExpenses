﻿VACUUM;

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
