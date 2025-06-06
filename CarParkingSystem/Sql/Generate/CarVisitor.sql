PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "CarVisitor";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "CarVisitor" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_no" TEXT,
    "car_no" TEXT,
    "client" INTEGER NOT NULL,
    "client_id" TEXT,
    "create_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "end_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "order_id" TEXT,
    "phone" TEXT,
    "remark" TEXT,
    "start_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "true_name" TEXT,
    "visitor_house" TEXT
);

