// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ticketer.Api.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected TicketerDbContext _context;
        public Repository(TicketerDbContext context)
        {
            this._context = context;
        }

        public Task Save() => _context.SaveChangesAsync();

        public async Task<bool> Any(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).AnyAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).CountAsync();
        }

        public async Task<T?> Create(T Entity)
        {
            await _context.Set<T>().AddAsync(Entity);
            return Entity;
        }

        public async Task<T?> Update(T Entity)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(Entity).State = EntityState.Modified;
            await Task.CompletedTask;
            return Entity;
        }

        public async Task Delete(T Entity)
        {
            _context.Set<T>().Remove(Entity);
            await Task.CompletedTask;
        }

        public async Task ExecuteScript(string SqlScript, params SqlParameter[] Parameters)
        {
            await _context.Database.ExecuteSqlRawAsync(SqlScript, Parameters);
        }

        public async Task<T?> FirstOrDefault()
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).ToListAsync();
        }

        public async Task<IEnumerable<T?>> FindWhere(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).ToListAsync();
        }

        public async Task<T?> Single(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(Predicate);
        }

        public async Task<IEnumerable<T?>> GetAll()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByID(int ID)
        {
            return await _context.Set<T>().FindAsync(ID);
        }

        public async Task<T?> GetByID(string ID)
        {
            return await _context.Set<T>().FindAsync(ID);
        }
    }
}
