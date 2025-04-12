using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkAreaDao : BaseDao<ParkArea>
    {

        public ParkAreaDao(AppDbContext context) : base(context)
        {
        }

    }
}

