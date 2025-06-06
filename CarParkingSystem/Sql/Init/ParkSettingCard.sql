PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkSettingCard" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_color" INTEGER NOT NULL,
    "card_no" INTEGER NOT NULL,
    "car_type" INTEGER NOT NULL,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

COMMIT;
PRAGMA foreign_keys = ON;


