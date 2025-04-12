using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class LongTermRentalCardTypeDao : BaseDao<LongTermRentalCardType>
    {

        public LongTermRentalCardTypeDao(AppDbContext context) : base(context)
        {
        }

    }
}

