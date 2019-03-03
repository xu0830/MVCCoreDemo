using CJ.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJ.Infrastructure.Repositories
{
    /// <summary>
    /// 仓储CRUD接口
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public interface IRepository<Entity> where Entity : IEntity
    {
        //public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        /// <summary>
        /// 返回全部数据
        /// </summary>
        /// <returns></returns>
        IQueryable<Entity> GetAll();

        /// <summary>
        /// 异步返回全部数据
        /// </summary>
        /// <returns></returns>
        Task<List<Entity>> GetAllAsync();

        /// <summary>
        /// 根据主键返回实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entity Get(int id);

        /// <summary>
        /// 根据主键异步数据表中的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Entity> GetAsync(int id);

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Entity Insert(Entity entity);

        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Entity> InsertAsync(Entity entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Entity Update(Entity entity);

        /// <summary>
        /// 异步更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Entity> UpdateAsync(Entity entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(Entity entity);

        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(Entity entity);
    }
}
