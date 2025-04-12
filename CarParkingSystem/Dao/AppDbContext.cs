using CarParkingSystem.Models;
using CarParkingSystem.Services;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Dao
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
public DbSet<FeeRuleSection> FeeRuleSections { get; set; }

public DbSet<ValueCar> ValueCars { get; set; }

public DbSet<LongTermRentalCar> LongTermRentalCars { get; set; }

public DbSet<ValueCardType> ValueCardTypes { get; set; }

public DbSet<LongTermRentalCardType> LongTermRentalCardTypes { get; set; }

public DbSet<ValueCardAction> ValueCardActions { get; set; }

public DbSet<DelayCardAction> DelayCardActions { get; set; }

public DbSet<FeeRule> FeeRules { get; set; }

public DbSet<BlackCar> BlackCars { get; set; }

public DbSet<CarFree> CarFrees { get; set; }

public DbSet<ParkingAbnormal> ParkingAbnormals { get; set; }

public DbSet<ParkingOutRecord> ParkingOutRecords { get; set; }

public DbSet<ParkingInRecord> ParkingInRecords { get; set; }

public DbSet<ParkWayStopTime> ParkWayStopTimes { get; set; }

public DbSet<ParkWayGroup> ParkWayGroups { get; set; }

public DbSet<ParkWayCard> ParkWayCards { get; set; }

public DbSet<ParkWay> ParkWays { get; set; }

public DbSet<ParkWayVoice> ParkWayVoices { get; set; }

public DbSet<ParkSetting> ParkSettings { get; set; }


public DbSet<ParkDevice> ParkDevices { get; set; }

public DbSet<OpenGateReason> OpenGateReasons { get; set; }

public DbSet<HandOver> HandOvers { get; set; }

public DbSet<UserWay> UserWays { get; set; }

public DbSet<OpenGateRecord> OpenGateRecords { get; set; }


public DbSet<Calendar> Calendars { get; set; }



public DbSet<CarTypePara> CarTypeParas { get; set; }

public DbSet<CarConvert> CarConverts { get; set; }


public DbSet<ParkSettingCard> ParkSettingCards { get; set; }

public DbSet<CarVisitor> CarVisitors { get; set; }

public DbSet<ParkingArrears> ParkingArrearss { get; set; }


public DbSet<OrderRefund> OrderRefunds { get; set; }

public DbSet<Order> Orders { get; set; }

public DbSet<Sum> Sums { get; set; }



public DbSet<ParkArea> ParkAreas { get; set; }

public DbSet<ParkInfo> ParkInfos { get; set; }


        public DbSet<CodeClass> CodeClasses { get; set; }
        public DbSet<CodeField> CodeFields { get; set; }


        public DbSet<Role> Roles { get; set; }
        public ILogService LogSerivce { get; }

        public AppDbContext(ILogService logService)
        {
            LogSerivce = logService;
            //如果不存在表，就先建表
            InitTables(this);//20250408
            //ExecuteSqlFile(this, "CarParkingSystem", "user");
            //ExecuteSqlFile(this, "CarParkingSystem", "role");
            //ExecuteSqlFile(this, "CarParkingSystem", "CodeClass");
            //ExecuteSqlFile(this, "CarParkingSystem", "CodeField");
        }

        private void InitTables(DbContext context)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string sqlDir = System.IO.Path.Combine(basePath, "CarParkingSystem", "Sql\\Init\\");
            var sqlFiles = Directory.GetFiles(sqlDir, "*.sql");
            foreach (string filePath in sqlFiles)
            {
                if (System.IO.File.Exists(filePath))
                {
                    var sql = System.IO.File.ReadAllText(filePath);

                    context.Database.ExecuteSqlRaw(sql);
                }
            }
        }

        private void ExecuteSqlFile(DbContext context, string projectName, string tableName)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            //string filename = item.ProjectName + "\\Sql\\" + item.TableName + "s.sql";//对应 EF 中 DbSet 的复数
            string filename = projectName + "\\Sql\\Init\\" + tableName + ".sql";//对应 EF 中 DbSet 的复数
            string filePath = System.IO.Path.Combine(basePath, filename);
            if (System.IO.File.Exists(filePath))
            {
                var sql = System.IO.File.ReadAllText(filePath);

                context.Database.ExecuteSqlRaw(sql);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //SQLitePCL.Batteries.Init();
#if DEBUG
            optionsBuilder.UseSqlite("Data Source=car_parking.db").LogTo(message => System.Diagnostics.Debug.WriteLine(message));
            //.LogTo(Console.WriteLine, LogLevel.Information);
#else
            optionsBuilder.UseSqlite("Data Source=car_parking.db");
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            BuildUserTable(modelBuilder);
BuildFeeRuleSectionTable(modelBuilder);

BuildValueCarTable(modelBuilder);

BuildLongTermRentalCarTable(modelBuilder);

BuildValueCardTypeTable(modelBuilder);

BuildLongTermRentalCardTypeTable(modelBuilder);

BuildValueCardActionTable(modelBuilder);

BuildDelayCardActionTable(modelBuilder);

BuildFeeRuleTable(modelBuilder);

BuildBlackCarTable(modelBuilder);

BuildCarFreeTable(modelBuilder);

BuildParkingAbnormalTable(modelBuilder);

BuildParkingOutRecordTable(modelBuilder);

BuildParkingInRecordTable(modelBuilder);

BuildParkWayStopTimeTable(modelBuilder);

BuildParkWayGroupTable(modelBuilder);

BuildParkWayCardTable(modelBuilder);

BuildParkWayTable(modelBuilder);

BuildParkWayVoiceTable(modelBuilder);

BuildParkSettingTable(modelBuilder);

BuildParkDeviceTable(modelBuilder);

BuildOpenGateReasonTable(modelBuilder);

BuildHandOverTable(modelBuilder);

BuildUserWayTable(modelBuilder);

BuildOpenGateRecordTable(modelBuilder);

BuildCalendarTable(modelBuilder);

BuildCarTypeParaTable(modelBuilder);

BuildCarConvertTable(modelBuilder);

BuildParkSettingCardTable(modelBuilder);

BuildCarVisitorTable(modelBuilder);

BuildParkingArrearsTable(modelBuilder);

BuildOrderRefundTable(modelBuilder);

BuildOrderTable(modelBuilder);

BuildSumTable(modelBuilder);

BuildParkAreaTable(modelBuilder);

BuildParkInfoTable(modelBuilder);


            BuildRoleTable(modelBuilder);//old
            BuildCodeClassTable(modelBuilder);
            BuildCodeFieldTable(modelBuilder);
        }
        private void BuildFeeRuleSectionTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeeRuleSection>((entity) =>
            {
                entity.ToTable("FeeRuleSection");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.FeeRuleFlag).HasColumnName("fee_rule_flag");
                entity.Property(p => p.FeeRuleId).HasColumnName("fee_rule_id");
                entity.Property(p => p.InWay).HasColumnName("in_way");
                entity.Property(p => p.OutWay).HasColumnName("out_way");
                entity.Property(p => p.OvertimeFeeRule).HasColumnName("overtime_fee_rule");
                entity.Property(p => p.OvertimeType).HasColumnName("overtime_type");
                entity.Property(p => p.ParkingFeeRule).HasColumnName("parking_fee_rule");
                entity.Property(p => p.ParkingTime).HasColumnName("parking_time");
                entity.Property(p => p.SectionName).HasColumnName("section_name");
            });
        }



        private void BuildValueCarTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueCar>((entity) =>
            {
                entity.ToTable("ValueCar");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.Balance).HasColumnName("balance");
                entity.Property(p => p.CarCode).HasColumnName("car_code");
                entity.Property(p => p.Card).HasColumnName("card");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.Deposit).HasColumnName("deposit");
                entity.Property(p => p.ParkSpace).HasColumnName("park_space");
                entity.Property(p => p.ParkSpaceType).HasColumnName("park_space_type");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.SpaceName).HasColumnName("space_name");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.Username).HasColumnName("username");
                entity.Property(p => p.UserRemark).HasColumnName("user_remark");
                entity.Property(p => p.UserTel).HasColumnName("user_tel");
                entity.Property(p => p.ValidEnd).HasColumnName("valid_end");
                entity.Property(p => p.ValidFrom).HasColumnName("valid_from");
            });
        }



        private void BuildLongTermRentalCarTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LongTermRentalCar>((entity) =>
            {
                entity.ToTable("LongTermRentalCar");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.Balance).HasColumnName("balance");
                entity.Property(p => p.CarCode).HasColumnName("car_code");
                entity.Property(p => p.Card).HasColumnName("card");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.Deposit).HasColumnName("deposit");
                entity.Property(p => p.ParkSpace).HasColumnName("park_space");
                entity.Property(p => p.ParkSpaceType).HasColumnName("park_space_type");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.SpaceName).HasColumnName("space_name");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.Username).HasColumnName("username");
                entity.Property(p => p.UserRemark).HasColumnName("user_remark");
                entity.Property(p => p.UserTel).HasColumnName("user_tel");
                entity.Property(p => p.ValidEnd).HasColumnName("valid_end");
                entity.Property(p => p.ValidFrom).HasColumnName("valid_from");
            });
        }



        private void BuildValueCardTypeTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueCardType>((entity) =>
            {
                entity.ToTable("ValueCardType");
                entity.Property(p => p.Amount).HasColumnName("amount");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardName).HasColumnName("card_name");
                entity.Property(p => p.CardType).HasColumnName("card_type");
                entity.Property(p => p.CarSpace).HasColumnName("car_space");
                entity.Property(p => p.EndDate).HasColumnName("end_date");
                entity.Property(p => p.ExpireCard).HasColumnName("expire_card");
                entity.Property(p => p.FeeRuleType).HasColumnName("fee_rule_type");
                entity.Property(p => p.InCheck).HasColumnName("in_check");
                entity.Property(p => p.MonthToTempDiscount).HasColumnName("month_to_temp_discount");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.StartDate).HasColumnName("start_date");
                entity.Property(p => p.TotalCar).HasColumnName("total_car");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildLongTermRentalCardTypeTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LongTermRentalCardType>((entity) =>
            {
                entity.ToTable("LongTermRentalCardType");
                entity.Property(p => p.Amount).HasColumnName("amount");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardName).HasColumnName("card_name");
                entity.Property(p => p.CardType).HasColumnName("card_type");
                entity.Property(p => p.CarSpace).HasColumnName("car_space");
                entity.Property(p => p.EndDate).HasColumnName("end_date");
                entity.Property(p => p.ExpireCard).HasColumnName("expire_card");
                entity.Property(p => p.FeeRuleType).HasColumnName("fee_rule_type");
                entity.Property(p => p.InCheck).HasColumnName("in_check");
                entity.Property(p => p.MonthToTempDiscount).HasColumnName("month_to_temp_discount");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.StartDate).HasColumnName("start_date");
                entity.Property(p => p.TotalCar).HasColumnName("total_car");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildValueCardActionTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueCardAction>((entity) =>
            {
                entity.ToTable("ValueCardAction");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarId).HasColumnName("car_id");
                entity.Property(p => p.OpDate).HasColumnName("op_date");
                entity.Property(p => p.OpKind).HasColumnName("op_kind");
                entity.Property(p => p.OpMoney).HasColumnName("op_money");
                entity.Property(p => p.OpNo).HasColumnName("op_no");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.ValidEnd).HasColumnName("valid_end");
                entity.Property(p => p.ValidFrom).HasColumnName("valid_from");
            });
        }



        private void BuildDelayCardActionTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DelayCardAction>((entity) =>
            {
                entity.ToTable("DelayCardAction");//DelayCardAction
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarId).HasColumnName("car_id");
                entity.Property(p => p.OpDate).HasColumnName("op_date");
                entity.Property(p => p.OpKind).HasColumnName("op_kind");
                entity.Property(p => p.OpMoney).HasColumnName("op_money");
                entity.Property(p => p.OpNo).HasColumnName("op_no");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.ValidEnd).HasColumnName("valid_end");
                entity.Property(p => p.ValidFrom).HasColumnName("valid_from");
            });
        }



        private void BuildFeeRuleTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeeRule>((entity) =>
            {
                entity.ToTable("FeeRule");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.FeeRuleData).HasColumnName("fee_rule_data");
                entity.Property(p => p.FeeRuleName).HasColumnName("fee_rule_name");
                entity.Property(p => p.FeeRuleType).HasColumnName("fee_rule_type");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildBlackCarTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlackCar>((entity) =>
            {
                entity.ToTable("BlackCar");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.EndTime).HasColumnName("end_time");
                entity.Property(p => p.Reason).HasColumnName("reason");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildCarFreeTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarFree>((entity) =>
            {
                entity.ToTable("CarFree");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.EndTime).HasColumnName("end_time");
                entity.Property(p => p.FreeDesc).HasColumnName("free_desc");
                entity.Property(p => p.FromTime).HasColumnName("from_time");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.UserAddr).HasColumnName("user_addr");
                entity.Property(p => p.Username).HasColumnName("username");
                entity.Property(p => p.UserPhone).HasColumnName("user_phone");
                entity.Property(p => p.UserType).HasColumnName("user_type");
                entity.Property(p => p.WxOpenId).HasColumnName("wx_open_id");
            });
        }



        private void BuildParkingAbnormalTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkingAbnormal>((entity) =>
            {
                entity.ToTable("ParkingAbnormal");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarColor).HasColumnName("car_color");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.CarType).HasColumnName("car_type");
                entity.Property(p => p.InCpChanged).HasColumnName("in_cp_changed");
                entity.Property(p => p.InImg).HasColumnName("in_img");
                entity.Property(p => p.InTime).HasColumnName("in_time");
                entity.Property(p => p.InType).HasColumnName("in_type");
                entity.Property(p => p.InWayId).HasColumnName("in_way_id");
                entity.Property(p => p.OrderId).HasColumnName("order_id");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildParkingOutRecordTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkingOutRecord>((entity) =>
            {
                entity.ToTable("ParkingOutRecord");
                entity.Property(p => p.AmountMoney).HasColumnName("amount_money");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.ChargeType).HasColumnName("charge_type");
                entity.Property(p => p.DiscountMoney).HasColumnName("discount_money");
                entity.Property(p => p.InCarColor).HasColumnName("in_car_color");
                entity.Property(p => p.InCarType).HasColumnName("in_car_type");
                entity.Property(p => p.InCpChanged).HasColumnName("in_cp_changed");
                entity.Property(p => p.InImg).HasColumnName("in_img");
                entity.Property(p => p.InOperatorId).HasColumnName("in_operator_id");
                entity.Property(p => p.InRemark).HasColumnName("in_remark");
                entity.Property(p => p.InTime).HasColumnName("in_time");
                entity.Property(p => p.InType).HasColumnName("in_type");
                entity.Property(p => p.InWayId).HasColumnName("in_way_id");
                entity.Property(p => p.OpenType).HasColumnName("open_type");
                entity.Property(p => p.OrderId).HasColumnName("order_id");
                entity.Property(p => p.OutCarColor).HasColumnName("out_car_color");
                entity.Property(p => p.OutCarType).HasColumnName("out_car_type");
                entity.Property(p => p.OutCpChanged).HasColumnName("out_cp_changed");
                entity.Property(p => p.OutImg).HasColumnName("out_img");
                entity.Property(p => p.OutOperatorId).HasColumnName("out_operator_id");
                entity.Property(p => p.OutTime).HasColumnName("out_time");
                entity.Property(p => p.OutType).HasColumnName("out_type");
                entity.Property(p => p.OutWayId).HasColumnName("out_way_id");
                entity.Property(p => p.PaidMoney).HasColumnName("paid_money");
                entity.Property(p => p.PlateId).HasColumnName("plate_id");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildParkingInRecordTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkingInRecord>((entity) =>
            {
                entity.ToTable("ParkingInRecord");
                entity.Property(p => p.AmountMoney).HasColumnName("amount_money");
                entity.Property(p => p.AutoPay).HasColumnName("auto_pay");
                entity.Property(p => p.AutoPayId).HasColumnName("auto_pay_id");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CalculateOutTime).HasColumnName("calculate_out_time");
                entity.Property(p => p.CarColor).HasColumnName("car_color");
                entity.Property(p => p.CardChangeTime).HasColumnName("card_change_time");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.CarStatus).HasColumnName("car_status");
                entity.Property(p => p.CarType).HasColumnName("car_type");
                entity.Property(p => p.DiscountMoney).HasColumnName("discount_money");
                entity.Property(p => p.IncpChanged).HasColumnName("incp_changed");
                entity.Property(p => p.InImg).HasColumnName("in_img");
                entity.Property(p => p.InOperatorId).HasColumnName("in_operator_id");
                entity.Property(p => p.InTime).HasColumnName("in_time");
                entity.Property(p => p.InType).HasColumnName("in_type");
                entity.Property(p => p.InWayId).HasColumnName("in_way_id");
                entity.Property(p => p.MonthToTempNumber).HasColumnName("month_to_temp_number");
                entity.Property(p => p.OpenType).HasColumnName("open_type");
                entity.Property(p => p.OrderId).HasColumnName("order_id");
                entity.Property(p => p.OriginCardNo).HasColumnName("origin_card_no");
                entity.Property(p => p.PaidMoney).HasColumnName("paid_money");
                entity.Property(p => p.PlateId).HasColumnName("plate_id");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildSumTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sum>((entity) =>
            {
                entity.ToTable("Sum");
                entity.Property(p => p.CarOutCount).HasColumnName("car_out_count");
                entity.Property(p => p.Cash).HasColumnName("cash");
                entity.Property(p => p.Chuzhi).HasColumnName("chuzhi");
                entity.Property(p => p.Chuzhika).HasColumnName("chuzhika");
                entity.Property(p => p.Discount).HasColumnName("discount");
                entity.Property(p => p.Dt).HasColumnName("dt");
                entity.Property(p => p.NeedPay).HasColumnName("need_pay");
                entity.Property(p => p.Paid).HasColumnName("paid");
                entity.Property(p => p.Pp).HasColumnName("pp");
                entity.Property(p => p.Yueka).HasColumnName("yueka");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
            });
        }



        private void BuildParkWayStopTimeTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkWayStopTime>((entity) =>
            {
                entity.ToTable("ParkWayStopTime");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardId).HasColumnName("card_id");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.StopEndHour).HasColumnName("stop_end_hour");
                entity.Property(p => p.StopEndMinute).HasColumnName("stop_end_minute");
                entity.Property(p => p.StopStartHour).HasColumnName("stop_start_hour");
                entity.Property(p => p.StopStartMinute).HasColumnName("stop_start_minute");
                entity.Property(p => p.WayId).HasColumnName("way_id");
                entity.Property(p => p.Weeks).HasColumnName("weeks");
            });
        }



        private void BuildParkWayGroupTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkWayGroup>((entity) =>
            {
                entity.ToTable("ParkWayGroup");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.BlankingTime).HasColumnName("blanking_time");
                entity.Property(p => p.NextWayId).HasColumnName("next_way_id");
                entity.Property(p => p.PreWayId).HasColumnName("pre_way_id");
            });
        }



        private void BuildParkWayCardTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkWayCard>((entity) =>
            {
                entity.ToTable("ParkWayCard");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardId).HasColumnName("card_id");
                entity.Property(p => p.IsConfirm).HasColumnName("is_confirm");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.WayId).HasColumnName("way_id");
            });
        }



        private void BuildParkWayTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkWay>((entity) =>
            {
                entity.ToTable("ParkWay");
                entity.Property(p => p.Amount).HasColumnName("amount");
                entity.Property(p => p.AreaId).HasColumnName("area_id");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarInId).HasColumnName("car_in_id");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.CarNoColor).HasColumnName("car_no_color");
                entity.Property(p => p.CarNoType).HasColumnName("car_no_type");
                entity.Property(p => p.CarStatus).HasColumnName("car_status");
                entity.Property(p => p.CarTypeId).HasColumnName("car_type_id");
                entity.Property(p => p.ChangedCarNo).HasColumnName("changed_car_no");
                entity.Property(p => p.Discount).HasColumnName("discount");
                entity.Property(p => p.Display).HasColumnName("display");
                entity.Property(p => p.InImage).HasColumnName("in_image");
                entity.Property(p => p.InTime).HasColumnName("in_time");
                entity.Property(p => p.IsAllowEnter).HasColumnName("is_allow_enter");
                entity.Property(p => p.IsCsConfirm).HasColumnName("is_cs_confirm");
                entity.Property(p => p.IsNeedAysn).HasColumnName("is_need_aysn");
                entity.Property(p => p.IsPaid).HasColumnName("is_paid");
                entity.Property(p => p.LastCarNo).HasColumnName("last_car_no");
                entity.Property(p => p.LastCarTime).HasColumnName("last_car_time");
                entity.Property(p => p.OrderId).HasColumnName("order_id");
                entity.Property(p => p.Paid).HasColumnName("paid");
                entity.Property(p => p.PlateId).HasColumnName("plate_id");
                entity.Property(p => p.RecStatus).HasColumnName("rec_status");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.SpecialCar).HasColumnName("special_car");
                entity.Property(p => p.TriggerFlag).HasColumnName("trigger_flag");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.VideoCall).HasColumnName("video_call");
                entity.Property(p => p.VideoCallQrcode).HasColumnName("video_call_qrcode");
                entity.Property(p => p.VideoCallTime).HasColumnName("video_call_time");
                entity.Property(p => p.Voice).HasColumnName("voice");
                entity.Property(p => p.WaitPay).HasColumnName("wait_pay");
                entity.Property(p => p.WaittingCarNo).HasColumnName("waitting_car_no");
                entity.Property(p => p.WaittingCarNoColor).HasColumnName("waitting_car_no_color");
                entity.Property(p => p.WaittingCarNoType).HasColumnName("waitting_car_no_type");
                entity.Property(p => p.WaittingImg).HasColumnName("waitting_img");
                entity.Property(p => p.WaittingPlateId).HasColumnName("waitting_plate_id");
                entity.Property(p => p.WaittingTime).HasColumnName("waitting_time");
                entity.Property(p => p.WayCarType).HasColumnName("way_car_type");
                entity.Property(p => p.WayConnect).HasColumnName("way_connect");
                entity.Property(p => p.WayName).HasColumnName("way_name");
                entity.Property(p => p.WayNo).HasColumnName("way_no");
                entity.Property(p => p.WayStatus).HasColumnName("way_status");
                entity.Property(p => p.WayType).HasColumnName("way_type");
            });
        }



        private void BuildParkWayVoiceTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkWayVoice>((entity) =>
            {
                entity.ToTable("ParkWayVoice");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.EndHour).HasColumnName("end_hour");
                entity.Property(p => p.EndMinute).HasColumnName("end_minute");
                entity.Property(p => p.LastUpdateDate).HasColumnName("last_update_date");
                entity.Property(p => p.StartHour).HasColumnName("start_hour");
                entity.Property(p => p.StartMinute).HasColumnName("start_minute");
                entity.Property(p => p.Volume).HasColumnName("volume");
                entity.Property(p => p.WayId).HasColumnName("way_id");
            });
        }



        private void BuildParkSettingTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkSetting>((entity) =>
            {
                entity.ToTable("ParkSetting");
                entity.Property(p => p.AbnormalSetting).HasColumnName("abnormal_setting");
                entity.Property(p => p.AutoMatch).HasColumnName("auto_match");
                entity.Property(p => p.CarUpperLimit).HasColumnName("car_upper_limit");
                entity.Property(p => p.CarUpperLimitProcess).HasColumnName("car_upper_limit_process");
                entity.Property(p => p.ChangeTempCar).HasColumnName("change_temp_car");
                entity.Property(p => p.DefaultCardId).HasColumnName("default_card_id");
                entity.Property(p => p.DelayBySpace).HasColumnName("delay_by_space");
                entity.Property(p => p.DelayTime).HasColumnName("delay_time");
                entity.Property(p => p.EntryWayWaittingCar).HasColumnName("entry_way_waitting_car");
                entity.Property(p => p.FreeTime).HasColumnName("free_time");
                entity.Property(p => p.IsNeedReason).HasColumnName("is_need_reason");
                entity.Property(p => p.IsSelfEntry).HasColumnName("is_self_entry");
                entity.Property(p => p.LeaveDate).HasColumnName("leave_date");
                entity.Property(p => p.MotorbikeDefaultCard).HasColumnName("motorbike_default_card");
                entity.Property(p => p.MulSpace).HasColumnName("mul_space");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.MulSpaceExpired).HasColumnName("mul_space_expired");
                entity.Property(p => p.OneLotMoreCarEnter).HasColumnName("one_lot_more_car_enter");
                entity.Property(p => p.OneLotMoreCarTempCar).HasColumnName("one_lot_more_car_temp_car");
                entity.Property(p => p.ParkingFull).HasColumnName("parking_full");
                entity.Property(p => p.ResCanOpenTime).HasColumnName("res_can_open_time");
                entity.Property(p => p.ShowTodayIncome).HasColumnName("show_today_income");
                entity.Property(p => p.TempCarManager).HasColumnName("temp_car_manager");
                entity.Property(p => p.UnlicensedModel).HasColumnName("unlicensed_model");
                entity.Property(p => p.UnsaveInAbnormal).HasColumnName("unsave_in_abnormal");
                entity.Property(p => p.UnsaveManualAbnormal).HasColumnName("unsave_manual_abnormal");
                entity.Property(p => p.UnsaveOutAbnormal).HasColumnName("unsave_out_abnormal");
                entity.Property(p => p.ValueCardDeduction).HasColumnName("value_card_deduction");
            });
        }





        private void BuildRoleTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>((entity) =>
            {
                entity.ToTable("Role");
                entity.Property(p => p.RoleId).HasColumnName("role_id");
                entity.Property(p => p.RoleName).HasColumnName("role_name");
                entity.Property(p => p.RoleRemark).HasColumnName("role_remark");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
            });
        }



        private void BuildParkDeviceTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkDevice>((entity) =>
            {
                entity.ToTable("ParkDevice");
                entity.Property(p => p.AllowCloseGate).HasColumnName("allow_close_gate");
                entity.Property(p => p.Camera0).HasColumnName("camera_0");
                entity.Property(p => p.Camera0Type).HasColumnName("camera_0_type");
                entity.Property(p => p.Camera1).HasColumnName("camera_1");
                entity.Property(p => p.Camera1Type).HasColumnName("camera_1_type");
                entity.Property(p => p.Camera485).HasColumnName("camera_485");
                entity.Property(p => p.CameraDoubleFilter).HasColumnName("camera_double_filter");
                entity.Property(p => p.CameraIo).HasColumnName("camera_io");
                entity.Property(p => p.CameraKey).HasColumnName("camera_key");
                entity.Property(p => p.CameraRecomeFilter).HasColumnName("camera_recome_filter");
                entity.Property(p => p.CardCameraIp).HasColumnName("card_camera_ip");
                entity.Property(p => p.CardCameraSn).HasColumnName("card_camera_sn");
                entity.Property(p => p.CardCameraType).HasColumnName("card_camera_type");
                entity.Property(p => p.CardPort).HasColumnName("card_port");
                entity.Property(p => p.CardIp).HasColumnName("card__ip");
                entity.Property(p => p.CardSn).HasColumnName("card_sn");
                entity.Property(p => p.CardType).HasColumnName("card_type");
                entity.Property(p => p.DevStatus).HasColumnName("dev_status");
                entity.Property(p => p.HasCard).HasColumnName("has_card");
                entity.Property(p => p.HasCarmera).HasColumnName("has_carmera");
                entity.Property(p => p.LedDisplay).HasColumnName("led_display");
                entity.Property(p => p.LedIp).HasColumnName("led_ip");
                entity.Property(p => p.LedType).HasColumnName("led_type");
                entity.Property(p => p.WayId).HasColumnName("way_id");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.QrCode).HasColumnName("qr_code");
                entity.Property(p => p.SetDisplay).HasColumnName("set_display");
                entity.Property(p => p.SetVoice).HasColumnName("set_voice");
                entity.Property(p => p.TwoGate).HasColumnName("two_gate");
            });
        }



        private void BuildOpenGateReasonTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OpenGateReason>((entity) =>
            {
                entity.ToTable("OpenGateReason");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.IsOut).HasColumnName("is_out");
                entity.Property(p => p.Reason).HasColumnName("reason");
            });
        }



        private void BuildHandOverTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HandOver>((entity) =>
            {
                entity.ToTable("HandOver");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.AbCar).HasColumnName("ab_car");
                entity.Property(p => p.ArrearsCar).HasColumnName("arrears_car");
                entity.Property(p => p.CashFee).HasColumnName("cash_fee");
                entity.Property(p => p.EndDate).HasColumnName("end_date");
                entity.Property(p => p.Etcfee).HasColumnName("etcfee");
                entity.Property(p => p.InCar).HasColumnName("in_car");
                entity.Property(p => p.IsFinished).HasColumnName("is_finished");
                entity.Property(p => p.OutCar).HasColumnName("out_car");
                entity.Property(p => p.PhoneFee).HasColumnName("phone_fee");
                entity.Property(p => p.StartDate).HasColumnName("start_date");
                entity.Property(p => p.TotalFee).HasColumnName("total_fee");
                entity.Property(p => p.UserId).HasColumnName("user_id");
                entity.Property(p => p.ValueCardFee).HasColumnName("value_card_fee");
            });
        }



        private void BuildUserWayTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserWay>((entity) =>
            {
                entity.ToTable("UserWay");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.UserId).HasColumnName("user_id");
                entity.Property(p => p.WayId).HasColumnName("way_id");
                entity.Property(p => p.OrderNo).HasColumnName("order_no");
            });
        }



        private void BuildOpenGateRecordTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OpenGateRecord>((entity) =>
            {
                entity.ToTable("OpenGateRecord");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CreateDate).HasColumnName("create_date");
                entity.Property(p => p.ImageUrl).HasColumnName("image_url");
                entity.Property(p => p.Reason).HasColumnName("reason");
                entity.Property(p => p.Username).HasColumnName("username");
                entity.Property(p => p.WayId).HasColumnName("way_id");
            });
        }






        private void BuildCalendarTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calendar>((entity) =>
            {
                entity.ToTable("Calendar");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CalendarName).HasColumnName("calendar_name");
                entity.Property(p => p.EndDate).HasColumnName("end_date");
                entity.Property(p => p.IsHoliday).HasColumnName("is_holiday");
                entity.Property(p => p.ParkingDate).HasColumnName("parking_date");
                entity.Property(p => p.StartDate).HasColumnName("start_date");
            });
        }




        private void BuildCarTypeParaTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarTypePara>((entity) =>
            {
                entity.ToTable("CarTypePara");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarTypeName).HasColumnName("car_type_name");
                entity.Property(p => p.Height).HasColumnName("height");
                entity.Property(p => p.Width).HasColumnName("width");
            });
        }



        private void BuildCarConvertTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarConvert>((entity) =>
            {
                entity.ToTable("CarConvert");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.ConvertCarNo).HasColumnName("convert_car_no");
            });
        }

        private void BuildParkSettingCardTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkSettingCard>((entity) =>
            {
                entity.ToTable("ParkSettingCard");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarColor).HasColumnName("car_color");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarType).HasColumnName("car_type");
                entity.Property(p => p.UpdateDt).HasColumnName("update_dt");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
            });
        }



        private void BuildCarVisitorTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarVisitor>((entity) =>
            {
                entity.ToTable("CarVisitor");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.Client).HasColumnName("client");
                entity.Property(p => p.ClientId).HasColumnName("client_id");
                entity.Property(p => p.CreateDate).HasColumnName("create_date");
                entity.Property(p => p.EndDate).HasColumnName("end_date");
                entity.Property(p => p.OrderId).HasColumnName("order_id");
                entity.Property(p => p.Phone).HasColumnName("phone");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.StartDate).HasColumnName("start_date");
                entity.Property(p => p.TrueName).HasColumnName("true_name");
                entity.Property(p => p.VisitorHouse).HasColumnName("visitor_house");
            });
        }



        private void BuildParkingArrearsTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkingArrears>((entity) =>
            {
                entity.ToTable("ParkingArrears");
                entity.Property(p => p.AmountMoney).HasColumnName("amount_money");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.CarColor).HasColumnName("car_color");
                entity.Property(p => p.CardNo).HasColumnName("card_no");
                entity.Property(p => p.CarNo).HasColumnName("car_no");
                entity.Property(p => p.CarOutId).HasColumnName("car_out_id");
                entity.Property(p => p.CarType).HasColumnName("car_type");
                entity.Property(p => p.DiscountMoney).HasColumnName("discount_money");
                entity.Property(p => p.Fee).HasColumnName("fee");
                entity.Property(p => p.InImg).HasColumnName("in_img");
                entity.Property(p => p.InOperatorId).HasColumnName("in_operator_id");
                entity.Property(p => p.InRemark).HasColumnName("in_remark");
                entity.Property(p => p.InTime).HasColumnName("in_time");
                entity.Property(p => p.InType).HasColumnName("in_type");
                entity.Property(p => p.InWayId).HasColumnName("in_way_id");
                entity.Property(p => p.OrderId).HasColumnName("order_id");
                entity.Property(p => p.OutImg).HasColumnName("out_img");
                entity.Property(p => p.OutOperatorId).HasColumnName("out_operator_id");
                entity.Property(p => p.OutRemark).HasColumnName("out_remark");
                entity.Property(p => p.OutTime).HasColumnName("out_time");
                entity.Property(p => p.OutType).HasColumnName("out_type");
                entity.Property(p => p.OutWayId).HasColumnName("out_way_id");
                entity.Property(p => p.PaidMoney).HasColumnName("paid_money");
            });
        }


        private void BuildOrderRefundTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderRefund>((entity) =>
            {
                entity.ToTable("OrderRefund");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.ProductType).HasColumnName("product_type");
                entity.Property(p => p.ProductId).HasColumnName("product_id");
                entity.Property(p => p.Buyer).HasColumnName("buyer");
                entity.Property(p => p.OrderMoney).HasColumnName("order_money");
                entity.Property(p => p.RefundType).HasColumnName("refund_type");
                entity.Property(p => p.RefundMoney).HasColumnName("refund_money");
                entity.Property(p => p.Reason).HasColumnName("reason");
                entity.Property(p => p.RefundStatus).HasColumnName("refund_status");
                entity.Property(p => p.RefundRemark).HasColumnName("refund_remark");
                entity.Property(p => p.CreateUser).HasColumnName("create_user");
                entity.Property(p => p.CreateDate).HasColumnName("create_date");
                entity.Property(p => p.PayOrder).HasColumnName("pay_order");
                entity.Property(p => p.RefundOrderId).HasColumnName("refund_order_id");
                entity.Property(p => p.RefundTransactionId).HasColumnName("refund_transaction_id");
                entity.Property(p => p.Merchant).HasColumnName("merchant");
                entity.Property(p => p.TransactionId).HasColumnName("transaction_id");
                entity.Property(p => p.ClientType).HasColumnName("client_type");
                entity.Property(p => p.ClientId).HasColumnName("client_id");
            });
        }



        private void BuildOrderTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>((entity) =>
            {
                entity.ToTable("Order");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.Merchant).HasColumnName("merchant");
                entity.Property(p => p.ProductType).HasColumnName("product_type");
                entity.Property(p => p.ProductId).HasColumnName("product_id");
                entity.Property(p => p.Buyer).HasColumnName("buyer");
                entity.Property(p => p.PayOrder).HasColumnName("pay_order");
                entity.Property(p => p.PayName).HasColumnName("pay_name");
                entity.Property(p => p.PayMoney).HasColumnName("pay_money");
                entity.Property(p => p.CreateDate).HasColumnName("create_date");
                entity.Property(p => p.PayStatus).HasColumnName("pay_status");
                entity.Property(p => p.PayType).HasColumnName("pay_type");
                entity.Property(p => p.ClientType).HasColumnName("client_type");
                entity.Property(p => p.ClientId).HasColumnName("client_id");
                entity.Property(p => p.PayUrl).HasColumnName("pay_url");
                entity.Property(p => p.ExpireDate).HasColumnName("expire_date");
                entity.Property(p => p.IsProfitSharing).HasColumnName("is_profit_sharing");
                entity.Property(p => p.TransactionId).HasColumnName("transaction_id");
                entity.Property(p => p.Remark).HasColumnName("remark");
                entity.Property(p => p.Phone).HasColumnName("phone");
                entity.Property(p => p.BuyNumber).HasColumnName("buy_number");
                entity.Property(p => p.StartTime).HasColumnName("start_time");
                entity.Property(p => p.EndTime).HasColumnName("end_time");
                entity.Property(p => p.Invoice).HasColumnName("invoice");
            });
        }


        private void BuildParkAreaTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkArea>((entity) =>
            {
                entity.ToTable("ParkArea");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.AreaName).HasColumnName("area_name");
                entity.Property(p => p.ShowAreaLot).HasColumnName("show_area_lot");
                entity.Property(p => p.TempCarFullCanIn).HasColumnName("temp_car_full_can_in");
                entity.Property(p => p.TotalCars).HasColumnName("total_cars");
                entity.Property(p => p.UpateUser).HasColumnName("upate_user");
                entity.Property(p => p.UpdateDate).HasColumnName("update_date");
                entity.Property(p => p.UsedCars).HasColumnName("used_cars");
            });
        }



        private void BuildParkInfoTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkInfo>((entity) =>
            {
                entity.ToTable("ParkInfo");
                entity.Property(p => p.Id).HasColumnName("aysn_id");
                entity.Property(p => p.PayType).HasColumnName("pay_type");
                entity.Property(p => p.UpdateUser).HasColumnName("update_user");
                entity.Property(p => p.UpdateDate).HasColumnName("update_date");
                entity.Property(p => p.PayTime).HasColumnName("pay_time");
                entity.Property(p => p.Merchant).HasColumnName("merchant");
                entity.Property(p => p.PayUuid).HasColumnName("pay_uuid");
                entity.Property(p => p.ParkUuid).HasColumnName("park_uuid");
                entity.Property(p => p.RemainingCars).HasColumnName("remaining_cars");
                entity.Property(p => p.TotalCars).HasColumnName("total_cars");
            });
        }




        private void BuildCodeClassTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CodeClass>((entity) =>
            {
                entity.ToTable("CodeClass")
                .HasMany(c => c.CodeFields)
                .WithOne(f => f.CodeClass)
                .HasForeignKey(f => f.Cid)
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.ProjectName).HasColumnName("project_name");
                entity.Property(p => p.TableName).HasColumnName("table_name");
                entity.Property(p => p.ClassName).HasColumnName("class_name");
            });
        }
        private void BuildCodeFieldTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CodeField>((entity) =>
            {
                entity.ToTable("CodeField");

                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Cid).HasColumnName("cid");
                entity.Property(p => p.FieldName).HasColumnName("field_name");
                entity.Property(p => p.FieldType).HasColumnName("field_type");
                entity.Property(p => p.FieldLength).HasColumnName("field_length");
                entity.Property(p => p.FieldRemark).HasColumnName("field_remark");
                entity.Property(p => p.IsMainKey).HasColumnName("is_main_key");
                entity.Property(p => p.IsAllowNull).HasColumnName("is_allow_null");
                entity.Property(p => p.IsAutoIncrement).HasColumnName("is_auto_increment");
                entity.Property(p => p.IsUnique).HasColumnName("is_unique");
                entity.Property(p => p.DefaultValue).HasColumnName("default_value");
            });
        }

        private void BuildUserTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>((entity) =>
            {
                entity.ToTable("user");

                entity.Ignore(p => p.IsSelected);//过滤的字段

                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Username).HasColumnName("username");
                entity.Property(p => p.Password).HasColumnName("password_hash");
                entity.Property(p => p.Salt).HasColumnName("salt");
                entity.Property(p => p.Email).HasColumnName("email");
                entity.Property(p => p.Phone).HasColumnName("phone");
                entity.Property(p => p.Status).HasColumnName("status");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at");
                entity.Property(p => p.UpdatedAt).HasColumnName("updated_at");
                entity.Property(p => p.LastLoginTime).HasColumnName("last_login_time");
            });
        }

       
    }
}
