using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkingInRecordDao : BaseDao<ParkingInRecord>
    {

        public ParkingInRecordDao(AppDbContext context) : base(context)
        {
        }

    }
}

