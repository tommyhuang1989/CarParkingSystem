PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;
DROP TABLE IF EXISTS ParkArea;
COMMIT;
PRAGMA foreign_keys = ON;

CREATE TABLE ParkArea (
    aysn_id INTEGER PRIMARY KEY AUTOINCREMENT,
    area_name TEXT NOT NULL,
    show_area_lot INTEGER,
    temp_car_full_can_in INTEGER,
    total_cars INTEGER,
    upate_user INTEGER NOT NULL,
    update_date TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    used_cars INTEGER
);

