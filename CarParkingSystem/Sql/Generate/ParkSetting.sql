PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS "ParkSetting";
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE "ParkSetting" (
    "abnormal_setting" INTEGER,
    "auto_match" INTEGER,
    "car_upper_limit" INTEGER,
    "car_upper_limit_process" INTEGER,
    "change_temp_car" INTEGER,
    "default_card_id" INTEGER,
    "delay_by_space" INTEGER,
    "delay_time" INTEGER,
    "entry_way_waitting_car" INTEGER,
    "free_time" INTEGER,
    "is_need_reason" INTEGER,
    "is_self_entry" INTEGER,
    "leave_date" INTEGER,
    "motorbike_default_card" INTEGER,
    "mul_space" INTEGER,
    "aysn_id" INTEGER PRIMARY KEY,
    "mul_space_expired" INTEGER,
    "one_lot_more_car_enter" INTEGER,
    "one_lot_more_car_temp_car" INTEGER,
    "parking_full" INTEGER,
    "res_can_open_time" INTEGER,
    "show_today_income" INTEGER,
    "temp_car_manager" INTEGER,
    "unlicensed_model" INTEGER,
    "unsave_in_abnormal" INTEGER,
    "unsave_manual_abnormal" INTEGER,
    "unsave_out_abnormal" INTEGER,
    "value_card_deduction" INTEGER
);

