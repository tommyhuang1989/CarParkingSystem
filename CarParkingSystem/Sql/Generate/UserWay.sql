PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "UserWay";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "UserWay" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "user_id" INTEGER NOT NULL,
    "way_id" INTEGER NOT NULL,
    "order_no" INTEGER
);

