using CarParkingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarParkingSystem.Dao
{
    public class UserDao : BaseDao<User>
    {

        public UserDao(AppDbContext context) : base(context)
        {
        }

        public User GetByUsername(string username)
        {
            return _context.Users?.FirstOrDefault(u => u.Username.Equals(username))!;
        }
        public User GetByEmail(string email)
        {
            return _context.Users?.FirstOrDefault(u => u.Email.Equals(email))!;
        }

        /// <summary>
        /// username 和 email 都应该唯一
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool? IsExists(string username, string email)
        {
            return _context.Users?.Any(u => u.Username.Equals(username) || u.Email.Equals(email))!;
        }

        /// <summary>
        /// 更新时，不能将 Username 和 Email 改为更其他记录相同；
        /// 自身更新时，id 是相同的，所以如果存在 Username 和 Email 相同，并且 id 不一致，就说明冲突了
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool? HasSameRecord(int id, string username, string email)
        {
            return _context.Users?.Any(u => u.Id != id && (u.Username.Equals(username) || u.Email.Equals(email)))!;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids">待删除的 ID 列表</param>
        //public async Task DeleteRange(List<int> ids)
        //{
        //    await _context.Users
        //        .Where(x => ids.Contains(x.Id))
        //        .ExecuteDeleteAsync();
        //}

    }
}
