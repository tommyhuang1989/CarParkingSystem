PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "Order" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "merchant" TEXT,
    "product_type" INTEGER NOT NULL,
    "product_id" TEXT,
    "buyer" TEXT NOT NULL,
    "pay_order" INTEGER,
    "pay_name" INTEGER NOT NULL,
    "pay_money" INTEGER NOT NULL,
    "create_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "pay_status" INTEGER NOT NULL,
    "pay_type" INTEGER NOT NULL,
    "client_type" INTEGER NOT NULL,
    "client_id" TEXT,
    "pay_url" TEXT,
    "expire_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "is_profit_sharing" INTEGER NOT NULL,
    "transaction_id" TEXT,
    "remark" TEXT,
    "phone" TEXT,
    "buy_number" INTEGER,
    "start_time" TEXT DEFAULT (datetime('now','localtime')),
    "end_time" TEXT DEFAULT (datetime('now','localtime')),
    "invoice" TEXT
);

COMMIT;
PRAGMA foreign_keys = ON;


