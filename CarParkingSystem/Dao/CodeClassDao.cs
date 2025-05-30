using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class CodeClassDao : BaseDao<CodeClass>
    {

        public CodeClassDao(AppDbContext context) : base(context)
        {
        }

        public CodeClass GetByTableName(string tableName)
        {
            return _context.CodeClasses?.FirstOrDefault(u => u.TableName.Equals(tableName))!;
        }
    }
}

