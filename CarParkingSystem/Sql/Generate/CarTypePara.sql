PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "CarTypePara";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "CarTypePara" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_no" INTEGER NOT NULL,
    "car_type_name" TEXT,
    "height" TEXT NOT NULL,
    "width" TEXT NOT NULL
);

