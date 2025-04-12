using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Dao
{
    public class BaseDao<T> where T : class
    {
        //public Expression<Func<User, object>> _expression { get; set; }
        protected readonly AppDbContext _context;

        public BaseDao(AppDbContext context)
        {
            _context = context;
        }

        // 新增
        public int Add(T entity)
        {
            int result = -1;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Set<T>().Add(entity);
                    result = _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _context.LogSerivce.LogError(Functions.GetExceptionMessage(e), "添加用户发生错误");
                    transaction.Rollback();
                    _context.ChangeTracker.Clear(); // 回滚后清除跟踪状态
                }
            }

            return result;
        }

        // 删除（通过主键）
        public void Delete(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids">待删除的 ID 列表</param>
        public async Task<int> DeleteRangeAsync<BaseEntity>(List<int> ids) where BaseEntity : class, IBaseEntity
        {
            return await _context.Set<BaseEntity>()
                .Where(x => ids.Contains(x.Id))
                .ExecuteDeleteAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>返回受影响的记录条数，如果是用原数据 update 可能会返回 0，所以 >=0 都可以认为是更新成功？</returns>
        public int Update(T entity)
        {
            int result = -1;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 更新操作
                    result = _context.SaveChanges();
                    _context.Entry(entity).State = EntityState.Modified;//更新成功后才修改状态
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _context.LogSerivce.LogError(Functions.GetExceptionMessage(e), "添加用户发生错误");
                    transaction.Rollback();
                    _context.ChangeTracker.Clear(); // 回滚后清除跟踪状态
                }
            }
            return result;

        }

        // 查询单个
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        // 查询全部
        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }
        public int Count(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null)
            {
                return _context.Set<T>().Count();
            }
            else
            {
                return _context.Set<T>().Count(filter);
            }
        }



        // 条件查询（使用 LINQ）
        public List<T> GetByCondition(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="currentPageIndex">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="orderBy"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAllPaged(
            int currentPageIndex, int pageSize,
            Expression<Func<T, object>> orderBy = null,
            bool isDescending = false,
            Expression<Func<T, bool>> filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query
                .Skip((currentPageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        /// <summary>
        /// 分页查询, 异步
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllPagedAsync(
            int currentPageIndex, int pageSize,
            Expression<Func<T, object>> orderBy = null,
            bool isDescending = false,
            Expression<Func<T, bool>> filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return await query
                .Skip((currentPageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
