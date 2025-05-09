PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "OrderRefund" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "product_type" INTEGER NOT NULL,
    "product_id" TEXT,
    "buyer" TEXT NOT NULL,
    "order_money" INTEGER NOT NULL,
    "refund_type" INTEGER NOT NULL,
    "refund_money" INTEGER NOT NULL,
    "reason" TEXT,
    "refund_status" INTEGER NOT NULL,
    "refund_remark" TEXT,
    "create_user" INTEGER NOT NULL,
    "create_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "pay_order" INTEGER,
    "refund_order_id" INTEGER,
    "refund_transaction_id" TEXT,
    "merchant" TEXT,
    "transaction_id" TEXT,
    "client_type" INTEGER NOT NULL,
    "client_id" TEXT
);

COMMIT;
PRAGMA foreign_keys = ON;


