PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Role";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "Role" (
    "role_id" INTEGER NOT NULL,
    "role_name" TEXT,
    "role_remark" TEXT,
    "aysn_id" INTEGER PRIMARY KEY
);

