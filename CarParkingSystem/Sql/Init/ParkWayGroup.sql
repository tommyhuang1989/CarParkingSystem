PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkWayGroup" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "blanking_time" INTEGER NOT NULL,
    "next_way_id" INTEGER NOT NULL,
    "pre_way_id" INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


