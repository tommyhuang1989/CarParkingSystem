using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class OpenGateReasonDao : BaseDao<OpenGateReason>
    {

        public OpenGateReasonDao(AppDbContext context) : base(context)
        {
        }

    }
}

