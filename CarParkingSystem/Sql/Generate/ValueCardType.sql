PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ValueCardType";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ValueCardType" (
    "amount" DECIMAL(19,4),
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_name" TEXT NOT NULL,
    "card_type" INTEGER,
    "car_space" INTEGER,
    "end_date" TEXT DEFAULT (datetime('now','localtime')),
    "expire_card" INTEGER,
    "fee_rule_type" INTEGER,
    "in_check" INTEGER,
    "month_to_temp_discount" TEXT,
    "rec_status" INTEGER,
    "remark" TEXT,
    "start_date" TEXT DEFAULT (datetime('now','localtime')),
    "total_car" INTEGER,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

