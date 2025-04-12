using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkingAbnormalDao : BaseDao<ParkingAbnormal>
    {

        public ParkingAbnormalDao(AppDbContext context) : base(context)
        {
        }

    }
}

