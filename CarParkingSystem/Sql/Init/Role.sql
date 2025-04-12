PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "Role" (
    "role_id" INTEGER NOT NULL,
    "role_name" TEXT,
    "role_remark" TEXT,
    "aysn_id" INTEGER PRIMARY KEY
);

COMMIT;
PRAGMA foreign_keys = ON;


