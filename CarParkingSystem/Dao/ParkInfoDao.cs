using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkInfoDao : BaseDao<ParkInfo>
    {

        public ParkInfoDao(AppDbContext context) : base(context)
        {
        }

    }
}

