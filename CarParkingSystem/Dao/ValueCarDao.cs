using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ValueCarDao : BaseDao<ValueCar>
    {

        public ValueCarDao(AppDbContext context) : base(context)
        {
        }

    }
}

