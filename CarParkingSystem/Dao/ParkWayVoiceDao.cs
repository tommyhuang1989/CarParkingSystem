using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class ParkWayVoiceDao : BaseDao<ParkWayVoice>
    {

        public ParkWayVoiceDao(AppDbContext context) : base(context)
        {
        }

    }
}

