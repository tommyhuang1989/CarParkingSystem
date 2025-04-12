using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkWayStopTimeDao : BaseDao<ParkWayStopTime>
    {

        public ParkWayStopTimeDao(AppDbContext context) : base(context)
        {
        }

    }
}

