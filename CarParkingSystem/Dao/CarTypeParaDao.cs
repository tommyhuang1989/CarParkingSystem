using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class CarTypeParaDao : BaseDao<CarTypePara>
    {

        public CarTypeParaDao(AppDbContext context) : base(context)
        {
        }

    }
}

