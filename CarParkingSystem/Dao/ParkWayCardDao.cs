using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkWayCardDao : BaseDao<ParkWayCard>
    {

        public ParkWayCardDao(AppDbContext context) : base(context)
        {
        }

    }
}

