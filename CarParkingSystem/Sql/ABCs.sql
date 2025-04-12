PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS ABCs;
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE ABCs (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
ParkingLotName TEXT NOT NULL,
RuleName TEXT NOT NULL,
RuleType TEXT NOT NULL,
RuleContent TEXT NOT NULL
);

