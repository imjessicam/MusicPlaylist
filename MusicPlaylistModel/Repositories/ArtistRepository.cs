using DTO.Models.Album;
using System.Linq;
using DTO.Models.Artist;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Repositories
{
    public class ArtistRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;

        public ArtistRepository(IDbContextFactory<DatabaseContext> factory)
        {
            _factory = factory;
        }

        public Guid Create(string name)
        {
            using var context = _factory.CreateDbContext();

            var externalId = Guid.NewGuid();

            context.Add(new Artist 
            { 
                ExternalId = externalId,
                Name = name,
            });

            context.SaveChanges();
            return externalId;
        }

        public IEnumerable<Guid> CreateMany(List<string> names)
        {
            using var context = _factory.CreateDbContext();

            var externalIds = new List<Guid>();

            if(names.Count > 0)
            {
                foreach (var name in names)
                {
                    var externalId = Guid.NewGuid();

                    context.Artists.Add(new Artist
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

        public Artist Find(Guid artistExternalId)
        {
            using var context = _factory.CreateDbContext();
            return context.Artists.FirstOrDefault(x => x.ExternalId == artistExternalId);            
        }

        public IEnumerable<Artist> FindMany(List<Guid> artistsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var foundArtists = context.Artists.Where(x => artistsExternalIds.Contains(x.ExternalId)).ToList();

            return foundArtists;
        }


        public Guid Edit(Guid artistExternalId, UpdateArtistModel artistToUpdate)
        {
            using var context = _factory.CreateDbContext();
            var artist = Find(artistExternalId);

            artist.Name = artistToUpdate.Name;

            context.Artists.Update(artist);
            context.SaveChanges();

            return artist.ExternalId;
        }

        public IEnumerable<Guid> EditMany(IReadOnlyList<UpdateArtistModel> artistsToUpdate)
        {
            // Sprawdzenie czy w podanej liscie albumow do zmiany nie znajduja sie duplikaty 
            var containsDuplicates = artistsToUpdate.GroupBy(x => x.Name).Select(x => x.Count()).FirstOrDefault(x => x > 1) == null;

            // Jesli duplikaty sie nie znajduja to przeprowadzamy kolejne operacje
            if (!containsDuplicates)
            {
                // zebranie podanych ExternalIds z albums do listy
                var artistsIds = artistsToUpdate.Select(x => x.ExternalId).ToList();

                using var context = _factory.CreateDbContext();

                // Wyszukanie w bazie danych albumow o podanych ExternalIds i zapisanie ich do listy
                var foundArtists = context.Artists.Where(x => artistsIds.Contains(x.ExternalId)).ToList();

                // Jesli liczba podanych elementow zgadza sie z liczba wyszukanych w bazie danych albumow - przeprowadzamy kolejne operacje 
                var isAllExist = foundArtists.Count == artistsToUpdate.Count;

                if (isAllExist)
                {
                    // polaczenie w pary wyszukiwanych elemntow ze znalezionymi odpowiednikami w bazie danych, oraz  
                    var margedTwoList = artistsToUpdate.Zip((foundArtists), (toUpdate, founded) => new { ToUpdate = toUpdate, Founded = founded }).ToList();

                    // zamiana "title" w bazie danych na nowe "title" 
                    foreach (var pairData in margedTwoList)
                    {
                        pairData.Founded.Name = pairData.ToUpdate.Name;
                        context.Update(pairData.Founded);
                    }

                    // zapisanie danych
                    context.SaveChanges();

                    // wyciagniecie ExternalIds edztowanych albumow
                    return foundArtists.Select(x => x.ExternalId);
                }
                return null;
            }
            return null;
        }

        public void Delete(Guid artistExternalId)
        {
            using var context = _factory.CreateDbContext();

            var artistToDelete = Find(artistExternalId);

            context.Remove(artistToDelete);
            context.SaveChanges();
        }

        public void DeleteMany(List<Guid> artistsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var artistsToDelete = FindMany(artistsExternalIds);

            context.RemoveRange(artistsToDelete);
            context.SaveChanges();
        }
    }
}
