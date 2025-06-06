PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkDevice";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkDevice" (
    "allow_close_gate" INTEGER,
    "camera_0" TEXT,
    "camera_0_type" INTEGER,
    "camera_1" TEXT,
    "camera_1_type" INTEGER,
    "camera_485" INTEGER,
    "camera_double_filter" INTEGER,
    "camera_io" INTEGER,
    "camera_key" TEXT,
    "camera_recome_filter" INTEGER,
    "card_camera_ip" TEXT,
    "card_camera_sn" TEXT,
    "card_camera_type" INTEGER,
    "card_port" INTEGER,
    "card__ip" TEXT,
    "card_sn" TEXT,
    "card_type" INTEGER,
    "dev_status" INTEGER NOT NULL,
    "has_card" INTEGER,
    "has_carmera" INTEGER,
    "led_display" TEXT,
    "led_ip" TEXT,
    "led_type" INTEGER,
    "way_id" INTEGER NOT NULL,
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "qr_code" INTEGER NOT NULL,
    "set_display" TEXT,
    "set_voice" TEXT,
    "two_gate" INTEGER NOT NULL
);

