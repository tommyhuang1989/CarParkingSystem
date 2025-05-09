PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "FeeRule";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "FeeRule" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "fee_rule_data" TEXT NOT NULL,
    "fee_rule_name" TEXT NOT NULL,
    "fee_rule_type" INTEGER,
    "rec_status" INTEGER NOT NULL,
    "update_dt" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER NOT NULL
);

