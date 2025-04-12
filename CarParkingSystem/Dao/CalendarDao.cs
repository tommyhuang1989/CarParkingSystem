using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class CalendarDao : BaseDao<Calendar>
    {

        public CalendarDao(AppDbContext context) : base(context)
        {
        }

    }
}

