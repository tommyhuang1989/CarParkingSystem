PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "UserWay" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "user_id" INTEGER NOT NULL,
    "way_id" INTEGER NOT NULL,
    "order_no" INTEGER
);

COMMIT;
PRAGMA foreign_keys = ON;


