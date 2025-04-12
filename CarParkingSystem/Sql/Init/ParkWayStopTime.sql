PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkWayStopTime" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_id" INTEGER NOT NULL,
    "remark" TEXT,
    "stop_end_hour" INTEGER NOT NULL,
    "stop_end_minute" INTEGER NOT NULL,
    "stop_start_hour" INTEGER NOT NULL,
    "stop_start_minute" INTEGER NOT NULL,
    "way_id" INTEGER NOT NULL,
    "weeks" TEXT NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


