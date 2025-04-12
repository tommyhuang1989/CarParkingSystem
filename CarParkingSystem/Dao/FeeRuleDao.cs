using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class FeeRuleDao : BaseDao<FeeRule>
    {

        public FeeRuleDao(AppDbContext context) : base(context)
        {
        }

    }
}

