PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "ParkingInRecord" (
    "amount_money" DECIMAL(19,4),
    "auto_pay" INTEGER,
    "auto_pay_id" TEXT,
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "calculate_out_time" TEXT DEFAULT (datetime('now','localtime')),
    "car_color" INTEGER,
    "card_change_time" TEXT DEFAULT (datetime('now','localtime')),
    "card_no" INTEGER,
    "car_no" TEXT NOT NULL,
    "car_status" INTEGER,
    "car_type" INTEGER,
    "discount_money" DECIMAL(19,4),
    "incp_changed" INTEGER NOT NULL,
    "in_img" TEXT,
    "in_operator_id" INTEGER,
    "in_time" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "in_type" INTEGER NOT NULL,
    "in_way_id" INTEGER,
    "month_to_temp_number" INTEGER,
    "open_type" INTEGER,
    "order_id" TEXT,
    "origin_card_no" INTEGER,
    "paid_money" DECIMAL(19,4),
    "plate_id" TEXT,
    "rec_status" INTEGER,
    "remark" TEXT,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

COMMIT;
PRAGMA foreign_keys = ON;


