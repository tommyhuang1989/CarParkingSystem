PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "CarConvert" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_no" TEXT,
    "convert_car_no" TEXT
);

COMMIT;
PRAGMA foreign_keys = ON;


