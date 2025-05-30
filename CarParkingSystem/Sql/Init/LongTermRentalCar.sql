PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "LongTermRentalCar" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "balance" DECIMAL(19,4),
    "car_code" TEXT,
    "card" INTEGER NOT NULL,
    "car_no" TEXT NOT NULL,
    "deposit" DECIMAL(19,4),
    "park_space" TEXT,
    "park_space_type" INTEGER,
    "rec_status" INTEGER,
    "remark" TEXT,
    "space_name" TEXT,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER,
    "username" TEXT,
    "user_remark" TEXT,
    "user_tel" TEXT,
    "valid_end" TEXT DEFAULT (datetime('now','localtime')),
    "valid_from" TEXT DEFAULT (datetime('now','localtime'))
);

COMMIT;
PRAGMA foreign_keys = ON;


