using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkingArrearsDao : BaseDao<ParkingArrears>
    {

        public ParkingArrearsDao(AppDbContext context) : base(context)
        {
        }

    }
}

