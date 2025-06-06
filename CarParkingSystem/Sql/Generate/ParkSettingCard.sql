PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkSettingCard";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkSettingCard" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_color" INTEGER NOT NULL,
    "card_no" INTEGER NOT NULL,
    "car_type" INTEGER NOT NULL,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

