using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class FeeRuleSectionDao : BaseDao<FeeRuleSection>
    {

        public FeeRuleSectionDao(AppDbContext context) : base(context)
        {
        }

    }
}

