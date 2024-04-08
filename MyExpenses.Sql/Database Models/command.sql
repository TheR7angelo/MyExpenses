CREATE VIEW v_value_by_month_year_category AS
SELECT  STRFTIME('%Y-%m', h.date) AS month_year, ct.name, ROUND(SUM(h.value), 2) AS total
FROM t_history h
         LEFT JOIN t_category_type ct
                   ON h.category_type_fk = ct.id
GROUP BY month_year, ct.name
ORDER BY month_year;

CREATE VIEW v_value_by_month_year AS
SELECT STRFTIME('%Y-%m', h.date) AS month_year, ROUND(SUM(h.value), 2) AS total
FROM t_history h
GROUP BY month_year
ORDER BY month_year;