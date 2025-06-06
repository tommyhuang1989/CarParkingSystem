PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkWayVoice" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "end_hour" INTEGER NOT NULL,
    "end_minute" INTEGER NOT NULL,
    "last_update_date" TEXT DEFAULT (datetime('now','localtime')),
    "start_hour" INTEGER NOT NULL,
    "start_minute" INTEGER NOT NULL,
    "volume" INTEGER,
    "way_id" INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


