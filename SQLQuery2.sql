UPDATE Instrumentos
SET Disponibilidade = 1
WHERE Id IN (SELECT InstrumentosId FROM CompraInstrumento);

DELETE FROM CompraInstrumento;
DELETE FROM Compras;