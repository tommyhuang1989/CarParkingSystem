using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class RoleDao : BaseDao<Role>
    {

        public RoleDao(AppDbContext context) : base(context)
        {
        }

    }
}

