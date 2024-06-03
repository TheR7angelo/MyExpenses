SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1 AND pointed = 1;

SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1 AND pointed = 0;

SELECT ROUND(SUM(value), 2)
FROM t_history
WHERE account_fk = 1;

ALTER TABLE t_history RENAME compte_fk TO account_fk;


SELECT STRFTIME('%Y-%m', th.date) AS date,
       th.account_fk,
       ROUND(SUM(th.value), 2) AS value
FROM t_history th
GROUP BY th.account_fk, STRFTIME('%Y-%m', th.date);
