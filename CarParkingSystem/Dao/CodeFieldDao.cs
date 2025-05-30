using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CarParkingSystem.Dao
{
    public class CodeFieldDao : BaseDao<CodeField>
    {

        public CodeFieldDao(AppDbContext context) : base(context)
        {
        }
        public CodeField GetByFieldName(string fieldName)
        {
            return _context.CodeFields?.FirstOrDefault(u => u.FieldName.Equals(fieldName))!;
        }
    }
}

