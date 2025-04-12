PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkWay" (
    "amount" DECIMAL(19,4),
    "area_id" INTEGER,
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_in_id" INTEGER,
    "car_no" TEXT,
    "car_no_color" INTEGER,
    "car_no_type" INTEGER,
    "car_status" INTEGER,
    "car_type_id" INTEGER,
    "changed_car_no" INTEGER,
    "discount" DECIMAL(19,4),
    "display" TEXT,
    "in_image" TEXT,
    "in_time" TEXT DEFAULT (datetime('now','localtime')),
    "is_allow_enter" INTEGER,
    "is_cs_confirm" INTEGER,
    "is_need_aysn" INTEGER,
    "is_paid" INTEGER NOT NULL,
    "last_car_no" TEXT,
    "last_car_time" TEXT DEFAULT (datetime('now','localtime')),
    "order_id" TEXT,
    "paid" DECIMAL(19,4),
    "plate_id" TEXT,
    "rec_status" INTEGER,
    "remark" TEXT,
    "special_car" INTEGER,
    "trigger_flag" INTEGER,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER,
    "video_call" INTEGER,
    "video_call_qrcode" INTEGER,
    "video_call_time" TEXT DEFAULT (datetime('now','localtime')),
    "voice" TEXT,
    "wait_pay" DECIMAL(19,4),
    "waitting_car_no" TEXT,
    "waitting_car_no_color" INTEGER,
    "waitting_car_no_type" INTEGER,
    "waitting_img" TEXT,
    "waitting_plate_id" TEXT,
    "waitting_time" TEXT DEFAULT (datetime('now','localtime')),
    "way_car_type" INTEGER,
    "way_connect" INTEGER,
    "way_name" TEXT NOT NULL,
    "way_no" TEXT NOT NULL,
    "way_status" INTEGER,
    "way_type" INTEGER NOT NULL
);

COMMIT;
PRAGMA foreign_keys = ON;


