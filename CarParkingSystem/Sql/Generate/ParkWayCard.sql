PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkWayCard";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkWayCard" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_id" INTEGER NOT NULL,
    "is_confirm" INTEGER,
    "rec_status" INTEGER,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER,
    "way_id" INTEGER NOT NULL
);

