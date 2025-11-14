using StudentsRegistrationSystem.Core.BaseEntity;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponseOffset<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}