using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class OpenGateRecordDao : BaseDao<OpenGateRecord>
    {

        public OpenGateRecordDao(AppDbContext context) : base(context)
        {
        }

    }
}

