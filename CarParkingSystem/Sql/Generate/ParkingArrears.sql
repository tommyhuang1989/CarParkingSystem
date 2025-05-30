PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkingArrears";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkingArrears" (
    "amount_money" TEXT,
    "aysn_id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "car_color" INTEGER,
    "card_no" INTEGER,
    "car_no" TEXT NOT NULL,
    "car_out_id" INTEGER,
    "car_type" INTEGER,
    "discount_money" TEXT,
    "fee" DECIMAL(19,4) NOT NULL,
    "in_img" TEXT,
    "in_operator_id" INTEGER,
    "in_remark" TEXT,
    "in_time" TEXT DEFAULT (datetime('now','localtime')),
    "in_type" INTEGER,
    "in_way_id" INTEGER,
    "order_id" TEXT NOT NULL,
    "out_img" TEXT,
    "out_operator_id" INTEGER,
    "out_remark" TEXT,
    "out_time" TEXT DEFAULT (datetime('now','localtime')),
    "out_type" INTEGER,
    "out_way_id" INTEGER,
    "paid_money" TEXT
);

