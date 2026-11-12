using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.DataBase.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? FindLogin(string email, string password)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
    }

    public bool VerifyIfExist(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }
}
