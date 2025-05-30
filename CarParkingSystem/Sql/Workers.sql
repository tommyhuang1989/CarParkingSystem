PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS Workers;
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE Workers (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Name TEXT NOT NULL UNIQUE,
CreatedAt TEXT DEFAULT (datetime('now','localtime')),
UpdatedAt TEXT DEFAULT (datetime('now','localtime'))
);

