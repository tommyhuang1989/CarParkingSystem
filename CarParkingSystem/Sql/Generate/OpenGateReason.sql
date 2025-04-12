PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "OpenGateReason";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "OpenGateReason" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "is_out" INTEGER NOT NULL,
    "reason" TEXT NOT NULL
);

