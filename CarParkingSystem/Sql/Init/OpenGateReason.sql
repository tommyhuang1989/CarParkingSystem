PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "OpenGateReason" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "is_out" INTEGER NOT NULL,
    "reason" TEXT NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


