PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "FeeRule" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "fee_rule_data" TEXT NOT NULL,
    "fee_rule_name" TEXT NOT NULL,
    "fee_rule_type" INTEGER,
    "rec_status" INTEGER NOT NULL,
    "update_dt" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


