using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class OrderDao : BaseDao<Order>
    {

        public OrderDao(AppDbContext context) : base(context)
        {
        }

    }
}

