using Data.Base;
using System.Linq.Expressions;

namespace Repository.Interfaces;

public interface IBaseRepository<TEntity>
	where TEntity : BaseEntity
{
	Task<TEntity?> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<TEntity>> GetAllAsync(
		CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<TEntity>> FindAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken = default);

	Task<bool> ExistsAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken = default);

	Task AddAsync(
		TEntity entity,
		CancellationToken cancellationToken = default);

	void Update(
		TEntity entity);

	void Delete(
		TEntity entity);

	Task<int> SaveChangesAsync(
		CancellationToken cancellationToken = default);
}