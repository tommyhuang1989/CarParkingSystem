PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "Calendar" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "calendar_name" TEXT,
    "end_date" TEXT DEFAULT (datetime('now','localtime')),
    "is_holiday" INTEGER NOT NULL,
    "parking_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "start_date" TEXT DEFAULT (datetime('now','localtime'))
);

COMMIT;
PRAGMA foreign_keys = ON;


