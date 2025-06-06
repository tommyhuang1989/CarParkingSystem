PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkWayCard" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_id" INTEGER NOT NULL,
    "is_confirm" INTEGER,
    "rec_status" INTEGER,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER,
    "way_id" INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


