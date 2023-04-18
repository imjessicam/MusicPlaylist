
using DTO.Models.Artist;
using System.Linq;
using DTO.Models.User;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Repositories
{
    public class UserRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;
        public UserRepository(IDbContextFactory<DatabaseContext> factory)
        {
            _factory = factory;
        }

        public Guid Create(string name)
        {
            using var context = _factory.CreateDbContext();

            var externalId = Guid.NewGuid();

            context.Add(new User 
            { 
                ExternalId = externalId,
                Name = name

            });

            context.SaveChanges();
            return externalId;
        }

        public IEnumerable<Guid> CreateMany(List<string> names)
        {
            using var context = _factory.CreateDbContext();

            var externalIds = new List<Guid>();

            if (names.Count > 0)
            {
                foreach (var name in names)
                {
                    var externalId = Guid.NewGuid();

                    context.Users.Add(new User
                    {
                        ExternalId = externalId,
                        Name = name

                    });
                    externalIds.Add(externalId);
                }
                context.SaveChanges();

                return externalIds;
            }

            return new List<Guid>();
        }

        public User Find(Guid userExternalId)
        {
            using var context = _factory.CreateDbContext();
            return context.Users.FirstOrDefault(x => x.ExternalId == userExternalId);
        }

        public IEnumerable<User> FindMany(List<Guid> userExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var foundUsers = context.Users.Where(x => userExternalIds.Contains(x.ExternalId)).ToList();

            return foundUsers;
        }


        public Guid Edit(Guid userExternalId, UpdateUserModel userToUpdate)
        {
            using var context = _factory.CreateDbContext();
            var user = Find(userExternalId);

            user.Name = userToUpdate.Name;

            context.Users.Update(user);
            context.SaveChanges();

            return user.ExternalId;
        }

        public IEnumerable<Guid> EditMany(IReadOnlyList<UpdateUserModel> usersToUpdate)
        {
            // Sprawdzenie czy w podanej liscie albumow do zmiany nie znajduja sie duplikaty 
            var containsDuplicates = usersToUpdate.GroupBy(x => x.Name).Select(x => x.Count()).FirstOrDefault(x => x > 1) == null;

            // Jesli duplikaty sie nie znajduja to przeprowadzamy kolejne operacje
            if (!containsDuplicates)
            {
                // zebranie podanych ExternalIds z albums do listy
                var usersIds = usersToUpdate.Select(x => x.ExternalId).ToList();

                using var context = _factory.CreateDbContext();

                // Wyszukanie w bazie danych albumow o podanych ExternalIds i zapisanie ich do listy
                var foundUsers = context.Users.Where(x => usersIds.Contains(x.ExternalId)).ToList();

                // Jesli liczba podanych elementow zgadza sie z liczba wyszukanych w bazie danych albumow - przeprowadzamy kolejne operacje 
                var isAllExist = foundUsers.Count == usersToUpdate.Count;

                if (isAllExist)
                {
                    // polaczenie w pary wyszukiwanych elemntow ze znalezionymi odpowiednikami w bazie danych, oraz  
                    var margedTwoList = usersToUpdate.Zip((foundUsers), (toUpdate, founded) => new { ToUpdate = toUpdate, Founded = founded }).ToList();

                    // zamiana "title" w bazie danych na nowe "title" 
                    foreach (var pairData in margedTwoList)
                    {
                        pairData.Founded.Name = pairData.ToUpdate.Name;
                        context.Update(pairData.Founded);
                    }

                    // zapisanie danych
                    context.SaveChanges();

                    // wyciagniecie ExternalIds edytowanych albumow
                    return foundUsers.Select(x => x.ExternalId);
                }
                return null;
            }
            return null;
        }

        public void Delete(Guid userExternalId)
        {
            using var context = _factory.CreateDbContext();

            var userToDelete = Find(userExternalId);

            context.Remove(userToDelete);
            context.SaveChanges();
        }

        public void DeleteMany(List<Guid> userExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var userToDelete = FindMany(userExternalIds);

            context.RemoveRange(userToDelete);
            context.SaveChanges();
        }

    }
    
}
