PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkingOutRecord";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkingOutRecord" (
    "amount_money" DECIMAL(19,4) NOT NULL,
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "card_no" INTEGER,
    "car_no" TEXT,
    "charge_type" INTEGER,
    "discount_money" DECIMAL(19,4) NOT NULL,
    "in_car_color" INTEGER,
    "in_car_type" INTEGER,
    "in_cp_changed" INTEGER,
    "in_img" TEXT,
    "in_operator_id" INTEGER,
    "in_remark" TEXT,
    "in_time" TEXT DEFAULT (datetime('now','localtime')),
    "in_type" INTEGER,
    "in_way_id" INTEGER NOT NULL,
    "open_type" INTEGER,
    "order_id" TEXT,
    "out_car_color" INTEGER,
    "out_car_type" INTEGER,
    "out_cp_changed" INTEGER,
    "out_img" TEXT,
    "out_operator_id" INTEGER,
    "out_time" TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    "out_type" INTEGER NOT NULL,
    "out_way_id" INTEGER NOT NULL,
    "paid_money" DECIMAL(19,4) NOT NULL,
    "plate_id" TEXT,
    "rec_status" INTEGER,
    "remark" TEXT,
    "update_dt" TEXT DEFAULT (datetime('now','localtime')),
    "update_user" INTEGER
);

