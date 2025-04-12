PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkWayGroup";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkWayGroup" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "blanking_time" INTEGER NOT NULL,
    "next_way_id" INTEGER NOT NULL,
    "pre_way_id" INTEGER NOT NULL
);

