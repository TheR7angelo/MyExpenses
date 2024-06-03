SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1 AND pointed = 1;

SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1;

ALTER TABLE t_history RENAME compte_fk TO account_fk;
