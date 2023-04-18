using DTO.Models.Song;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Repositories
{
    public class SongRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;
        private readonly AlbumRepository _albumRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ArtistRepository _artistRepository;

        public SongRepository(IDbContextFactory<DatabaseContext> factory, AlbumRepository albumRepository, CategoryRepository categoryRepository, ArtistRepository artistRepository)
        {
            _factory = factory;
            _albumRepository = albumRepository;
            _categoryRepository = categoryRepository;
            _artistRepository = artistRepository;
        }

        public Guid Create(string title, Guid albumExternalId, Guid categoryExternalId, IReadOnlyList<Guid> artistExternalIds)
        {
            using var context = _factory.CreateDbContext();

            // find Album
            var album = context.Albums.Single(x => x.ExternalId == albumExternalId);
            if (album == null)
            {
                throw new ArgumentOutOfRangeException("Album does not exist, please create.", innerException: null);
            }

            // find Category
            var category = context.Categories.Single(x => x.ExternalId == categoryExternalId);
            if (album == null)
            {
                throw new ArgumentOutOfRangeException("Category does not exist, please create.", innerException: null);
            }

            // find Artists
            var artists = context.Artists.Where(x => artistExternalIds.Contains(x.ExternalId)).ToList();

            if (artistExternalIds.Count == artists.Count)
            {
                var externalId = Guid.NewGuid();

                // Table Songs
                context.Songs.Add(new Song
                {
                    ExternalId = externalId,
                    Title = title,
                    AlbumId = album.Id,
                    CategoryId = category.Id,

                });
                context.SaveChanges();

                var song = context.Songs.Single(x => x.ExternalId == externalId);

                // Table ArtistsSongs
                foreach (var artist in artists)
                {
                    context.ArtistsSongs.Add(new ArtistSong
                    {
                        SongId = song.Id,
                        ArtistId = artist.Id,
                    });
                }
                context.SaveChanges();

                return externalId;
            }
            return Guid.Empty;
        }

        // CreateMany - Many songs.Title, but the same album, and category, and artists
        public IEnumerable<Guid> CreateManyInOneAlbum(List<string> titles, Guid albumExternalId, Guid categoryExternalId, IReadOnlyList<Guid> artistExternalIds)
        {
            using var context = _factory.CreateDbContext();            

            var songsExternalIds = new List<Guid>();

            // Foreach Title in entered Titles:
            foreach (var title in titles)
            {
                // Table Songs

                // - find Album
                var album = context.Albums.Single(x => x.ExternalId == albumExternalId);
                if (album == null)
                {
                    throw new ArgumentOutOfRangeException("Album does not exist, please create.", innerException: null);
                }

                // - find Category
                var category = context.Categories.Single(x => x.ExternalId == categoryExternalId);
                if (album == null)
                {
                    throw new ArgumentOutOfRangeException("Category does not exist, please create.", innerException: null);
                }              

                var externalId = Guid.NewGuid();

                context.Songs.Add(new Song
                {
                    ExternalId = externalId,
                    Title = title,
                    AlbumId = album.Id,
                    CategoryId = category.Id,                    

                });
                // Utworzenie listy externalIds z kazdego utworzonego tytyulu
                songsExternalIds.Add(externalId);                
            }
            context.SaveChanges();

            // Table ArtistsSongs            

            foreach (var songExternalId in songsExternalIds)
            {
                // - find Artists
                var artists = context.Artists.Where(x => artistExternalIds.Contains(x.ExternalId)).ToList();
                if (artists.Count() == 0)
                {
                    throw new ArgumentOutOfRangeException("Artists do not exist, please create", innerException: null);
                }

                var song = context.Songs.Single(x => x.ExternalId == songExternalId);

                foreach (var artist in artists)
                {
                    context.ArtistsSongs.Add(new ArtistSong
                    {
                        SongId = song.Id,
                        ArtistId = artist.Id,
                    });
                }
                context.SaveChanges();
            }
            return songsExternalIds;
        }

        public IEnumerable<Guid> CreateMany(IReadOnlyList<CreateSongModel> songsToAdd)
        {
            using var context = _factory.CreateDbContext();

            var songsExternalIds = new List<Guid>();
            var artistsInSong = new List<Artist>();

            foreach (var songToAdd in songsToAdd)
            {
                // find Album
                var album = _albumRepository.Find(songToAdd.AlbumId);
                if (album == null)
                {
                    throw new ArgumentOutOfRangeException("Album does not exist, please create.", innerException: null);
                }

                // find Category
                var category = _categoryRepository.Find(songToAdd.CategoryId);
                if (category == null)
                {
                    throw new ArgumentOutOfRangeException("Category does not exist, please create.", innerException: null);
                }

                var externalId = Guid.NewGuid();

                // Table Songs
                context.Songs.Add(new Song
                {
                    ExternalId = externalId,
                    Title = songToAdd.Title,
                    AlbumId = album.Id,
                    CategoryId = category.Id,

                });
                songsExternalIds.Add(externalId);

                context.SaveChanges();

                // - find artists to add in one Song
                var artistsLists = _artistRepository.FindMany(songToAdd.ArtistsIds).ToList();

                // Table ArtistsSongs
                // - find added song
                var song = context.Songs.Single(x => x.ExternalId == externalId);

                foreach(var artist in artistsLists)
                {
                    context.ArtistsSongs.Add(new ArtistSong 
                    { 
                        SongId = song.Id, 
                        ArtistId = artist.Id 
                    });
                }
                context.SaveChanges();
            }             
            return songsExternalIds;
        }

        public Album GetAlbum(Guid songExternalId)
        {
            using var context = _factory.CreateDbContext();
            var song = context
                .Songs
                .Include(x => x.Album)
                .Single(x => x.ExternalId == songExternalId);

            return song.Album;
        }

        public Category GetCategory(Guid songExternalId)
        {
            using var context = _factory.CreateDbContext();
            var song = context
                .Songs
                .Include(x => x.Category)
                .Single(x => x.ExternalId == songExternalId);

            return song.Category;
        }

        public IReadOnlyList<Artist> GetArtists(Guid songExternalId)
        {
            using var context = _factory.CreateDbContext();
            var song = context
                .Songs
                .Include(x => x.ArtistsSongs)
                .ThenInclude(x => x.Artist)
                .First(x => x.ExternalId == songExternalId);

            return song.ArtistsSongs.Select(x => x.Artist).ToList();
        }

        public Song Find(Guid songExternalId)
        {
            using var context = _factory.CreateDbContext();

            return context.Songs.FirstOrDefault(x => x.ExternalId == songExternalId);
        }

        public IEnumerable<Guid> FindMany(List<Guid> songsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var foundSongs = context.Songs.Where(x => songsExternalIds.Contains(x.ExternalId)).ToList();

            return foundSongs.Select(x => x.ExternalId);
        }

        public Guid Edit(Guid songExternalId, UpdateSongModel songToUpdate, IReadOnlyList<Guid> artistsExternalIds )
        {
            using var context = _factory.CreateDbContext();

            // Table Songs
            // - Find song

            var song = Find(songExternalId);

            // - Find album
            var album = _albumRepository.Find(songToUpdate.AlbumId);

            // - Find category
            var category = _categoryRepository.Find(songToUpdate.CategoryId);

            // - Change Title/Album/Category of Song 
            song.Title = songToUpdate.Title;
            song.AlbumId = album.Id;
            song.CategoryId = category.Id;

            // Update song
            context.Update(song);

            // Save changes
            context.SaveChanges();

            // Table ArtistsSongs - Remove Song
            // - Find Song in ArtistsSongs
            var songToDelete = context.ArtistsSongs.Where(x => x.SongId == song.Id).ToList();

            // - Delete Song in ArtistsSongs
            context.ArtistsSongs.RemoveRange(songToDelete);

            // - Save changes
            context.SaveChanges();

            // Table ArtistsSongs - Add Artists - Song
            // - Check if new artistsIds exist
            var foundArtists = context.Artists.Where(x => artistsExternalIds.Contains(x.ExternalId)).ToList();
            if(foundArtists == null)
            {
                throw new ArgumentOutOfRangeException("Artists do not exist, please create.", innerException: null);
            }

            // - Add new pairs Artists - Songs
            if (foundArtists.Count == artistsExternalIds.Count)
            {
                foreach (var artist in foundArtists)
                {
                    context.ArtistsSongs.Add(new ArtistSong
                    {
                        SongId = song.Id,
                        ArtistId = artist.Id,
                    });
                }
                context.SaveChanges();
            }

            return song.ExternalId;
        }

        public IEnumerable<Guid> EditMany(IReadOnlyList<UpdateSongModel> songsToUpdate)
        {

            // Sprawdzenie czy w podanej liscie albumow do zmiany nie znajduja sie duplikaty 
            var containsDuplicates = songsToUpdate.GroupBy(x => x.Title).Select(x => x.Count()).FirstOrDefault(x => x > 1) == null;

            // Jesli duplikaty sie nie znajduja to przeprowadzamy kolejne operacje
            if (!containsDuplicates)
            {
                // zebranie podanych ExternalIds z songs do listy
                var songsIds = songsToUpdate.Select(x => x.ExternalId).ToList();

                using var context = _factory.CreateDbContext();

                // Wyszukanie w bazie danych songs o podanych ExternalIds i zapisanie ich do listy
                var foundSongs = context.Songs.Where(x => songsIds.Contains(x.ExternalId)).ToList();

                // Jesli liczba podanych elementow zgadza sie z liczba wyszukanych w bazie danych songs - przeprowadzamy kolejne operacje 
                var isAllExist = foundSongs.Count == songsToUpdate.Count;

                if (isAllExist)
                {

                    // polaczenie w pary wyszukiwanych elemntow ze znalezionymi odpowiednikami w bazie danych, oraz  
                    var margedTwoList = songsToUpdate.Zip((foundSongs), (toUpdate, founded) => new { ToUpdate = toUpdate, Founded = founded }).ToList();

                    // zamiana "title" w bazie danych na nowe "title" 
                    foreach (var pairData in margedTwoList)
                    {
                        var album = _albumRepository.Find(pairData.ToUpdate.AlbumId);
                        var category = _categoryRepository.Find(pairData.ToUpdate.CategoryId);

                        pairData.Founded.Title = pairData.ToUpdate.Title;
                        pairData.Founded.AlbumId = album.Id;
                        pairData.Founded.CategoryId = category.Id;
                        context.Update(pairData.Founded);

                        var songToDelete = context.ArtistsSongs.Where(x => x.SongId == pairData.Founded.Id).ToList();
                        context.ArtistsSongs.RemoveRange(songToDelete);

                        context.SaveChanges();

                        var artistsExternalIds = _artistRepository.FindMany(pairData.ToUpdate.ArtistsIds).ToList();

                        //var foundArtists = context.Artists.Where(x => artistsExternalIds.Contains(x.ExternalId)).ToList();

                        foreach(var artist in artistsExternalIds)
                        {
                            context.ArtistsSongs.Add(new ArtistSong 
                            { 
                                ArtistId = artist.Id, 
                                SongId = pairData.Founded.Id 

                            });
                        }
                        context.SaveChanges();

                    }
                    // zapisanie danych
                    context.SaveChanges();

                    // wyciagniecie ExternalIds edztowanych albumow
                    return foundSongs.Select(x => x.ExternalId);

                }
                return null;

            }
            return null;

        }

        public void Delete(Guid songExternalId)
        {
            using var context = _factory.CreateDbContext();

            var songToDelete = Find(songExternalId);

            context.Remove(songToDelete);
            context.SaveChanges();
        }

        public void DeleteMany(List<Guid> songsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var songsToDelete = context.Songs.Where(x => songsExternalIds.Contains(x.ExternalId)).ToList();

            context.RemoveRange(songsToDelete);
            context.SaveChanges();
        }
    }
}
