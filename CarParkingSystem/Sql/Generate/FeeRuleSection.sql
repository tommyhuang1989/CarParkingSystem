PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "FeeRuleSection";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "FeeRuleSection" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "fee_rule_flag" INTEGER,
    "fee_rule_id" INTEGER NOT NULL,
    "in_way" INTEGER NOT NULL,
    "out_way" INTEGER NOT NULL,
    "overtime_fee_rule" INTEGER NOT NULL,
    "overtime_type" INTEGER NOT NULL,
    "parking_fee_rule" INTEGER NOT NULL,
    "parking_time" INTEGER NOT NULL,
    "section_name" TEXT
);

