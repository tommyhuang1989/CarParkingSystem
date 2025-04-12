using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ValueCardTypeDao : BaseDao<ValueCardType>
    {

        public ValueCardTypeDao(AppDbContext context) : base(context)
        {
        }

    }
}

