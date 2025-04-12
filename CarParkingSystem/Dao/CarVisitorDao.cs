using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class CarVisitorDao : BaseDao<CarVisitor>
    {

        public CarVisitorDao(AppDbContext context) : base(context)
        {
        }

    }
}

