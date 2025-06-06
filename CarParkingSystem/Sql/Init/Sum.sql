PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "Sum" (
    "car_out_count" INTEGER,
    "cash" DECIMAL(19,4),
    "chuzhi" DECIMAL(19,4),
    "chuzhika" DECIMAL(19,4),
    "discount" DECIMAL(19,4),
    "dt" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "need_pay" DECIMAL(19,4),
    "paid" DECIMAL(19,4),
    "pp" DECIMAL(19,4),
    "yueka" DECIMAL(19,4),
    "aysn_id" INTEGER PRIMARY KEY
);

COMMIT;
PRAGMA foreign_keys = ON;


