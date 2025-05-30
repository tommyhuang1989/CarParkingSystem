PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS Fees;
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE Fees (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
CarType TEXT NOT NULL,
LicensePlate TEXT NOT NULL,
PaymentAmount NUMERIC(10,2),
PaymentDateTime TEXT DEFAULT (datetime('now','localtime')),
PaymentChannel TEXT NOT NULL,
PaymentMethod TEXT NOT NULL,
LaneName TEXT NOT NULL,
OrderNumber TEXT NOT NULL,
CreatedAt TEXT DEFAULT (datetime('now','localtime'))
);

