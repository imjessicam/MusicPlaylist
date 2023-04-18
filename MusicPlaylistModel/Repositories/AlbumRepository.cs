
using System.Security.Cryptography.X509Certificates;
using DTO.Models.Album;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Repositories
{
    public class AlbumRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;

        public AlbumRepository(IDbContextFactory<DatabaseContext> factory)
        {
            _factory = factory;
        }

        public Guid Create(string title)
        {
            using var context = _factory.CreateDbContext();
                        
            var externalId = Guid.NewGuid();

            context.Add(new Album
            {
                ExternalId = externalId,
                Title = title

            });

            context.SaveChanges();

            return externalId;
        }

        public IEnumerable<Guid> CreateMany(List<string> titles)
        {
            using var context = _factory.CreateDbContext();

            var externalIds = new List<Guid>();

            if(titles.GroupBy(x => x).Where(g => g.Count() > 1).Count() == 0 && titles.Count > 0)
            {
                foreach (var title in titles)
                {
                    var externalId = Guid.NewGuid();
                    context.Add(new Album
                    {
                        ExternalId = externalId,
                        Title = title

                    });
                    externalIds.Add(externalId);
                }
                context.SaveChanges();

                return externalIds;
            }

            return Enumerable.Empty<Guid>();            
        }

        public Album Find(Guid albumExternalId)
        {
            using var context = _factory.CreateDbContext();
            var foundAlbum = context.Albums.Single(x => x.ExternalId == albumExternalId);

            return foundAlbum;
        }

        public IEnumerable<Album> FindMany(List<Guid> albumsExternalIds)
        {
            using var context = _factory.CreateDbContext();
            var foundAlbums = context.Albums.Where(x => albumsExternalIds.Contains(x.ExternalId)).ToList();

            return foundAlbums;           
        }

        public Guid Edit(Guid albumExternalId, UpdateAlbumModel albumToUpdate)
        {
            using var context = _factory.CreateDbContext();

            var album = Find(albumExternalId);

            album.Title = albumToUpdate.Title;

            context.Update(album);
            context.SaveChanges();

            return album.ExternalId;
        }

        public IEnumerable<Guid> EditMany(IReadOnlyList<UpdateAlbumModel> albumsToUpdate)
        {
            // Sprawdzenie czy w podanej liscie albumow do zmiany nie znajduja sie duplikaty 
            var containsDuplicates = albumsToUpdate.GroupBy(x => x.Title).Select(x => x.Count()).FirstOrDefault(x => x > 1) == null;

            // Jesli duplikaty sie nie znajduja to przeprowadzamy kolejne operacje
            if(!containsDuplicates)
            {
                // zebranie podanych ExternalIds z albums do listy
                var albumsIds = albumsToUpdate.Select(x => x.ExternalId).ToList();

                using var context = _factory.CreateDbContext();

                // Wyszukanie w bazie danych albumow o podanych ExternalIds i zapisanie ich do listy
                var foundAlbums = context.Albums.Where(x => albumsIds.Contains(x.ExternalId)).ToList();

                // Jesli liczba podanych elementow zgadza sie z liczba wyszukanych w bazie danych albumow - przeprowadzamy kolejne operacje 
                var isAllExist = foundAlbums.Count == albumsToUpdate.Count;

                if(isAllExist)
                {
                    // polaczenie w pary wyszukiwanych elemntow ze znalezionymi odpowiednikami w bazie danych, oraz  
                    var margedTwoList = albumsToUpdate.Zip((foundAlbums), (toUpdate, founded) => new { ToUpdate = toUpdate, Founded = founded }).ToList();

                    // zamiana "title" w bazie danych na nowe "title" 
                    foreach (var pairData in margedTwoList)
                    {
                        pairData.Founded.Title = pairData.ToUpdate.Title;
                        context.Update(pairData.Founded);
                    }

                    // zapisanie danych
                    context.SaveChanges();

                    // wyciagniecie ExternalIds edztowanych albumow
                    return foundAlbums.Select(x => x.ExternalId);
                }
                return null;
            }
            return null;
        }

        public void Delete(Guid albumExternalId)
        {
            using var context = _factory.CreateDbContext();

            var albumToDelete = Find(albumExternalId);

            context.Albums.Remove(albumToDelete);

            context.SaveChanges();
        }

        public void DeleteMany(List<Guid> albumsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var albumsToDelete = FindMany(albumsExternalIds);

            context.Albums.RemoveRange(albumsToDelete);

            context.SaveChanges();
        }
    }
}
