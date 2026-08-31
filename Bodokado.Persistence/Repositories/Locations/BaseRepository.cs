using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Application.Common.Pagination;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Repositories;

public class BaseRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected DbSet<T> DbSet => _context.Set<T>();

    public BaseRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<T>> GetPagedAsync(PaginationQuery query, Expression<Func<T, bool>>? predicate = null)
    {
        var normalized = query.Normalize();
        IQueryable<T> source = DbSet.AsQueryable();
        if (predicate != null) source = source.Where(predicate);
        var totalCount = await source.CountAsync();
        var items = await source.Skip(normalized.Skip).Take(normalized.Take).ToListAsync();
        return PagedResult<T>.Create(items, normalized, totalCount);
    }

    public async Task<List<T>> GetAllAsync() => await DbSet.ToListAsync();
    public async Task<int> CountAsync() => await DbSet.CountAsync();
    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) => await DbSet.Where(predicate).ToListAsync();
    public async Task<T?> GetByIdAsync(object id) => await DbSet.FindAsync(id);
    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);
    public void Update(T entity) => DbSet.Update(entity);
    public void Delete(T entity) => DbSet.Remove(entity);
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
