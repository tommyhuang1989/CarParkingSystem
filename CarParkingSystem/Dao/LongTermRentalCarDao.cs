using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class LongTermRentalCarDao : BaseDao<LongTermRentalCar>
    {

        public LongTermRentalCarDao(AppDbContext context) : base(context)
        {
        }

    }
}

