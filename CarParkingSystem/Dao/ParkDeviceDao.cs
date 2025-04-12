using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkDeviceDao : BaseDao<ParkDevice>
    {

        public ParkDeviceDao(AppDbContext context) : base(context)
        {
        }

    }
}

