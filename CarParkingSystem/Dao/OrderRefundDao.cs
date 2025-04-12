using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class OrderRefundDao : BaseDao<OrderRefund>
    {

        public OrderRefundDao(AppDbContext context) : base(context)
        {
        }

    }
}

