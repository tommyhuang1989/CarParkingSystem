PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "CarFree" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_no" TEXT NOT NULL,
    "end_time" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "free_desc" TEXT,
    "from_time" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "rec_status" INTEGER,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER,
    "user_addr" TEXT,
    "username" TEXT,
    "user_phone" TEXT,
    "user_type" INTEGER,
    "wx_open_id" TEXT
);

COMMIT;
PRAGMA foreign_keys = ON;


