using Microsoft.EntityFrameworkCore;
using Talkative.Infrastructure.Context;

namespace Talkative.Infrastructure.Repositories.Abstraction;

public class BaseRepository<T> where T : class 
{
    protected readonly ApplicationContext Context;

    public BaseRepository(ApplicationContext context)
    {
        Context = context;
    }
    
    public async Task<T?> AddAsync(T entity)
    {
        try
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();

            return entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task DeleteAsync(T entity)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }
}