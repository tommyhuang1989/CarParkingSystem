using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class CarFreeDao : BaseDao<CarFree>
    {

        public CarFreeDao(AppDbContext context) : base(context)
        {
        }

    }
}

