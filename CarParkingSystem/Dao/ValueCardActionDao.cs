using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ValueCardActionDao : BaseDao<ValueCardAction>
    {

        public ValueCardActionDao(AppDbContext context) : base(context)
        {
        }

    }
}

