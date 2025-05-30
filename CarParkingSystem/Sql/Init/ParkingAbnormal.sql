PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkingAbnormal" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_color" INTEGER,
    "card_no" INTEGER,
    "car_no" TEXT NOT NULL,
    "car_type" INTEGER,
    "in_cp_changed" INTEGER,
    "in_img" TEXT,
    "in_time" TEXT DEFAULT (datetime('now','localtime')),
    "in_type" INTEGER,
    "in_way_id" INTEGER,
    "order_id" TEXT,
    "rec_status" INTEGER,
    "remark" TEXT,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

COMMIT;
PRAGMA foreign_keys = ON;


