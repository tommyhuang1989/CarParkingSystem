using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class DelayCardActionDao : BaseDao<DelayCardAction>
    {

        public DelayCardActionDao(AppDbContext context) : base(context)
        {
        }

    }
}

