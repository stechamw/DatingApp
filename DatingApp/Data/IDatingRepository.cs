using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        //add a type of T in this case user or add a type of photo
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        //then create a method to save our changes
        Task<bool> SaveAll();
        //another task to get a list of users
        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
    }
}