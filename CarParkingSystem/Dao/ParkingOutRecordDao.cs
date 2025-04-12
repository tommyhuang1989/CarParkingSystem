using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkingOutRecordDao : BaseDao<ParkingOutRecord>
    {

        public ParkingOutRecordDao(AppDbContext context) : base(context)
        {
        }

    }
}

