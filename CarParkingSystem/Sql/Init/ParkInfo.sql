PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS ParkInfo (
    aysn_id INTEGER PRIMARY KEY AUTOINCREMENT,
    pay_type INTEGER NOT NULL,
    update_user INTEGER NOT NULL,
    update_date TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    pay_time INTEGER,
    merchant TEXT NOT NULL,
    pay_uuid TEXT NOT NULL,
    park_uuid TEXT NOT NULL,
    remaining_cars INTEGER NOT NULL,
    total_cars INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


