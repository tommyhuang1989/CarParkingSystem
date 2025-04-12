using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkWayDao : BaseDao<ParkWay>
    {

        public ParkWayDao(AppDbContext context) : base(context)
        {
        }

    }
}

