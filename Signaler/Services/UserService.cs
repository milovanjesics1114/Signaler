using Signaler.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signaler.Services
{
    public interface IUserService
    {

        public void RegisterUser(string username, string password);

        public bool UserExists(string username, string password);

        public User GetUser(string username);

        public List<User> All();
    }

    public class UserExistsException : Exception
    {
        public UserExistsException(string username) : base($"User with the username '{username}' already exists") { }
    }

     static class UserServiceValidator
    {
        public static void NoUserExists(SignalerDbContext dbContext, string username)
        {
            if (dbContext.User.Where(u => u.Username.Equals(username)) != null)
            {
                throw new UserExistsException(username);
            }
        }
        public static void ValidPassword(string password)
        {

        }
    }

    public class UserService : IUserService
    {       
        private readonly SignalerDbContext _dbContext;

        public UserService(SignalerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void RegisterUser(string username, string password)
        {
            // Check whether user exists.
            UserServiceValidator.NoUserExists(_dbContext, username);
            UserServiceValidator.ValidPassword(password);

            _dbContext.Add(new User { Username = username, Password = password });
            _dbContext.SaveChanges();
        }

        public bool UserExists(string username, string password)
        {
            return _dbContext.User.Where(u => u.Username == username && u.Password == password).FirstOrDefault() != null;
        }

        public User GetUser(string username)
        {
            return _dbContext.User.Where(u => u.Username == username).FirstOrDefault();
        }

        public List<User> All()
        {
            return _dbContext.User.ToList();
        }
    }
}
