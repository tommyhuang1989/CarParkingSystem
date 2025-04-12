using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class HandOverDao : BaseDao<HandOver>
    {

        public HandOverDao(AppDbContext context) : base(context)
        {
        }

    }
}

