PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "CarConvert";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "CarConvert" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_no" TEXT,
    "convert_car_no" TEXT
);

