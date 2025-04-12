using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class SumDao : BaseDao<Sum>
    {

        public SumDao(AppDbContext context) : base(context)
        {
        }

    }
}

