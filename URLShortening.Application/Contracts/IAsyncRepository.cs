using System.Linq.Expressions;
using URLShortening.Domain;

namespace URLShortening.Application.Contracts
{
    public interface IAsyncRepository<T> where T : EntityBase
    {
        Task<T?> GetById(int id);
        Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAll();
        Task<T> Insert(T model);
        Task<T> Update(T entity);
        Task<bool> Any(Expression<Func<T, bool>> predicate);
    }
}
