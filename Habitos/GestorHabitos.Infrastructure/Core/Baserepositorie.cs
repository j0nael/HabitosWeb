using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorHabitos.Infrastructure.Core;

public class Baserepositorie<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Baserepositorie(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        var existing = await _context.Set<T>().FindAsync(id);

        if (existing == null)
            throw new Exception("No encontrado");

        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<T> DeleteAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        _context.Set<T>().Remove(entity!);
        await _context.SaveChangesAsync();
        return entity!;
    }
}
