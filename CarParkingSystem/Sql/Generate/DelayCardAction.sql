PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "DelayCardAction";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "DelayCardAction" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_id" INTEGER NOT NULL,
    "op_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "op_kind" INTEGER NOT NULL,
    "op_money" DECIMAL(19,4) NOT NULL,
    "op_no" TEXT NOT NULL,
    "rec_status" INTEGER,
    "remark" TEXT,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER,
    "valid_end" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "valid_from" TEXT NOT NULL DEFAULT (datetime('now','localtime'))
);

