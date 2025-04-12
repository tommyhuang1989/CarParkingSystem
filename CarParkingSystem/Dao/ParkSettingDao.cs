using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkSettingDao : BaseDao<ParkSetting>
    {

        public ParkSettingDao(AppDbContext context) : base(context)
        {
        }

    }
}

