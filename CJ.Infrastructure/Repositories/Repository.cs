using CJ.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CJ.Infrastructure.Repositories
{
    public class Repository<Entity> : IRepository<Entity> where Entity: class, IEntity 
    {
        private DbContext Context { get; }

        public virtual DbSet<Entity> Table => Context.Set<Entity>();

        public Repository(DbContext _Context)
        {
            Context = _Context;
        }

        

        public void Delete(Entity entity)
        {
            Table.Remove(entity);
        }

        public Task DeleteAsync(Entity entity)
        {
            Delete(entity);
            return Task.FromResult(0);
        }

        public Entity Get(int id)
        {
            return Table.Where(c => c.Id == id).FirstOrDefault();
        }

        public Task<Entity> GetAsync(int id)
        {
            return Table.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public IQueryable<Entity> GetAll()
        {
            return Table;
        }

        public Task<List<Entity>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public Entity Insert(Entity entity)
        {
            return Table.Add(entity).Entity;
        }

        public Task<Entity> InsertAsync(Entity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        public Entity Update(Entity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public Task<Entity> UpdateAsync(Entity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }
    }
}
