PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "BlackCar";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "BlackCar" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_no" TEXT NOT NULL,
    "end_time" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "reason" TEXT,
    "rec_status" INTEGER,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

