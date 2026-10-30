SELECT CPF, COUNT(*) AS qtd FROM Usuarios GROUP BY CPF HAVING COUNT(*) > 1;
SELECT NomeUsuario, COUNT(*) AS qtd FROM Usuarios GROUP BY NomeUsuario HAVING COUNT(*) > 1;
SELECT Email, COUNT(*) AS qtd FROM Usuarios GROUP BY Email HAVING COUNT(*) > 1;
SELECT [Identificação], COUNT(*) AS qtd FROM Instrumentos GROUP BY [Identificação] HAVING COUNT(*) > 1;