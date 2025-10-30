// ===================================THIS FILE WAS AUTO GENERATED===================================

using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace Ticketer.Api.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T?>> GetAll();
        Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> Predicate);
        Task<T?> GetByID(int ID);
        Task<T?> GetByID(string ID);
        Task<bool> Any(Expression<Func<T, bool>> Predicate);
        Task<T?> FirstOrDefault();
        Task Save();
        Task<T?> Create(T Entity);
        Task<T?> Update(T Entity);
        Task Delete(T Entity);
        Task<int> Count(Expression<Func<T, bool>> Predicate);
        Task<T?> Single(Expression<Func<T, bool>> Predicate);
        Task ExecuteScript(string SqlScript, params SqlParameter[] Parameters);
        Task<IEnumerable<T?>> FindWhere(Expression<Func<T, bool>> Predicate);
    }
}
