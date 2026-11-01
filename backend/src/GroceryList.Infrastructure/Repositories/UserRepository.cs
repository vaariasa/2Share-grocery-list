using GroceryList.Application.Interfaces;
using GroceryList.Domain.Entities;
using GroceryList.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GroceryList.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username) =>
        context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<bool> UsernameExistsAsync(string username) =>
        context.Users.AnyAsync(u => u.Username == username);

    public async Task AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
