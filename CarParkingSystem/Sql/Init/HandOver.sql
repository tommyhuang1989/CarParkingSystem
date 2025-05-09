PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "HandOver" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "ab_car" INTEGER,
    "arrears_car" INTEGER,
    "cash_fee" DECIMAL(19,4),
    "end_date" TEXT DEFAULT (datetime('now','localtime')),
    "etcfee" DECIMAL(19,4),
    "in_car" INTEGER,
    "is_finished" INTEGER NOT NULL,
    "out_car" INTEGER,
    "phone_fee" DECIMAL(19,4),
    "start_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "total_fee" DECIMAL(19,4),
    "user_id" INTEGER NOT NULL,
    "value_card_fee" DECIMAL(19,4)
);

COMMIT;
PRAGMA foreign_keys = ON;


