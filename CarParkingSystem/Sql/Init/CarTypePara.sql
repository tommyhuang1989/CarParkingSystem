PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "CarTypePara" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_no" INTEGER NOT NULL,
    "car_type_name" TEXT,
    "height" TEXT NOT NULL,
    "width" TEXT NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


