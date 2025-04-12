using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class UserWayDao : BaseDao<UserWay>
    {

        public UserWayDao(AppDbContext context) : base(context)
        {
        }

    }
}

