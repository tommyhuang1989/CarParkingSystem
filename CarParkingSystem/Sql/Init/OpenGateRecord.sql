PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "OpenGateRecord" (
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "create_date" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "image_url" TEXT NOT NULL,
    "reason" TEXT,
    "username" TEXT NOT NULL,
    "way_id" INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


