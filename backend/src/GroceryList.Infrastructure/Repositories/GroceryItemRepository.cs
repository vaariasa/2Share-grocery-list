using GroceryList.Application.Interfaces;
using GroceryList.Domain.Entities;
using GroceryList.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GroceryList.Infrastructure.Repositories;

/// <summary>
/// Every query filters by UserId, so items of other users are never loaded.
/// </summary>
public class GroceryItemRepository(AppDbContext context) : IGroceryItemRepository
{
    public Task<List<GroceryItem>> GetAllByUserAsync(Guid userId) =>
        context.GroceryItems
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.CreatedAt)
            .ToListAsync();

    public Task<GroceryItem?> GetByIdAsync(Guid id, Guid userId) =>
        context.GroceryItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

    public async Task AddAsync(GroceryItem item)
    {
        context.GroceryItems.Add(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GroceryItem item)
    {
        context.GroceryItems.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(GroceryItem item)
    {
        context.GroceryItems.Remove(item);
        await context.SaveChangesAsync();
    }
}
