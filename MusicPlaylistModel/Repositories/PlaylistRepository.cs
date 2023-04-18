using System.Linq;
using DTO.Models.Playlist;
using DTO.Models.Song;
using DTO.Models.User;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Repositories
{
    public class PlaylistRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;
        private readonly UserRepository _userRepository;
        private readonly SongRepository _songRepository;

        public PlaylistRepository(IDbContextFactory<DatabaseContext> factory, UserRepository userRepository, SongRepository songRepository)
        {
            _factory = factory;
            _userRepository = userRepository;
            _songRepository = songRepository;
        }

        public Guid Create(string name, Guid userExternalId, IReadOnlyList<Guid> songExternalIds)
        {
            using var context = _factory.CreateDbContext();

            // find User
            var user = context.Users.Single(x => x.ExternalId == userExternalId);
            if(user == null)
            {
                throw new ArgumentOutOfRangeException("User does not exist, please create.", innerException: null);

            }

            // find Songs
            var songs = context.Songs.Where(x => songExternalIds.Contains(x.ExternalId)).ToList();
            if(songExternalIds.Count == songs.Count)
            {
                var externalId = Guid.NewGuid();

                // table Playlists
                context.Playlists.Add(new Playlist 
                { 
                    ExternalId = externalId,
                    Name = name,    
                    UserId = user.Id,
                });
                context.SaveChanges();

                var playlist = context.Playlists.Single(x => x.ExternalId == externalId);

                // table PlaylistsSongs
                foreach(var song in songs)
                {
                    context.PlaylistsSongs.Add(new PlaylistSong 
                    { 
                        PlaylistId = playlist.Id,
                        SongId = song.Id,
                    });
                }
                context.SaveChanges();

                return externalId;
            }

            return Guid.Empty;
        }

        public IEnumerable<Guid> CreateMany(IReadOnlyList<CreatePlaylistModel> playlistsToAdd)
        {
            using var context = _factory.CreateDbContext();

            var playlistsExternalIds = new List<Guid>();
            var songsInPlaylist = new List<Song>();

            foreach(var playlistToAdd in playlistsToAdd)
            {
                // find User
                var user = _userRepository.Find(playlistToAdd.UserId);
                if (user == null)
                {
                    throw new ArgumentOutOfRangeException("User does not exist, please create.", innerException: null);
                }

                // Table playlist
                var externalId = Guid.NewGuid();

                // - create new Playlist (externalId, Name, UserId)
                context.Playlists.Add(new Playlist 
                { 
                    ExternalId = externalId, 
                    Name = playlistToAdd.Name, 
                    UserId = user.Id 
                });

                // - create List of playlists ExternalIds
                playlistsExternalIds.Add(externalId);

                // - save changes
                context.SaveChanges();

                // - find Songs to add in one Playlist
                var songsExternalIdsLists = _songRepository.FindMany(playlistToAdd.SongsIds).ToList();
                var songsLists = context.Songs.Where(x => songsExternalIdsLists.Contains(x.ExternalId)).ToList();

                // Table PlaylistsSongs
                // - find added playlist
                var playlist = context.Playlists.Single(x => x.ExternalId == externalId);

                foreach(var song in songsLists)
                {
                    context.PlaylistsSongs.Add(new PlaylistSong
                    {
                        PlaylistId = playlist.Id,
                        SongId = song.Id
                    });
                }
                context.SaveChanges();
            }
            return playlistsExternalIds;
        }


        public User GetUser(Guid playlistExternalId)
        {
            using var context = _factory.CreateDbContext();

            var playlist = context
                .Playlists
                .Include(x => x.User)
                .FirstOrDefault(x => x.ExternalId ==  playlistExternalId);

            return playlist.User;
        }

        public IReadOnlyList<Song> GetSongs(Guid playlistExternalId)
        {
            using var context = _factory.CreateDbContext();

            var playlist = context
                .Playlists
                .Include(x => x.PlaylistsSongs)
                .ThenInclude(x => x.Song)
                .FirstOrDefault(x => x.ExternalId == playlistExternalId);

            return playlist.PlaylistsSongs.Select(x => x.Song).ToList();
        }

        public Playlist Find(Guid playlistExternalId)
        {
            using var context = _factory.CreateDbContext();

            return context.Playlists.FirstOrDefault(x => x.ExternalId == playlistExternalId);
        }

        public IEnumerable<Guid> FindMany(List<Guid> playlistsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var foundPlaylists = context.Playlists.Where(x => playlistsExternalIds.Contains(x.ExternalId)).ToList();

            return foundPlaylists.Select(x => x.ExternalId);
        }

        public Guid Edit(Guid playlistExternalId, UpdatePlaylistModel playlistToUpdate, IReadOnlyList<Guid> songsExternalIds)
        {
            var playlist = Find(playlistExternalId);

            // Find User
            var user = _userRepository.Find(playlistToUpdate.UserId);

            // Change Name, User and Department of User
            playlist.Name = playlistToUpdate.Name;
            playlist.UserId = user.Id;

            using var context = _factory.CreateDbContext();

            context.Update(playlist);
            context.SaveChanges();

            // Table PlaylistsSongs - Remove Playlist
            // - find Playlist in PlaylistsSongs
            var playlistToDelete = context.PlaylistsSongs.Where(x => x.PlaylistId == playlist.Id).ToList();

            // - delete Playlist in PlaylistsSongs
            context.PlaylistsSongs.RemoveRange(playlistToDelete);

            // - save changes
            context.SaveChanges();

            // Table PlaylistsSongs - Add Playlist - Song
            // - Check if new songsIds exist
            var foundSongs = context.Songs.Where(x => songsExternalIds.Contains(x.ExternalId)).ToList();
            if(foundSongs == null)
            {
                throw new ArgumentOutOfRangeException("Songs do not exist, please create.", innerException: null);
            }

            // - add new pairs Songs - Playlists
            if (foundSongs.Count == songsExternalIds.Count)
            {
                foreach (var song in foundSongs)
                {
                    context.PlaylistsSongs.Add(new PlaylistSong
                    {
                        PlaylistId = playlist.Id,
                        SongId = song.Id,
                    });
                }
                context.SaveChanges();
            }

            return playlist.ExternalId;
        }

        public IEnumerable<Guid> EditMany(IReadOnlyList<UpdatePlaylistModel> playlistsToUpdate)
        {

            // Sprawdzenie czy w podanej liscie playlists do zmiany nie znajduja sie duplikaty 
            var containsDuplicates = playlistsToUpdate.GroupBy(x => x.Name).Select(x => x.Count()).FirstOrDefault(x => x > 1) == null;

            // Jesli duplikaty sie nie znajduja to przeprowadzamy kolejne operacje
            if (!containsDuplicates)
            {
                // zebranie podanych ExternalIds z playlists do listy
                var playlistsIds = playlistsToUpdate.Select(x => x.ExternalId).ToList();

                using var context = _factory.CreateDbContext();

                // Wyszukanie w bazie danych playlists o podanych ExternalIds i zapisanie ich do listy
                var foundPlaylists = context.Playlists.Where(x => playlistsIds.Contains(x.ExternalId)).ToList();

                // Jesli liczba podanych elementow zgadza sie z liczba wyszukanych w bazie danych playlists - przeprowadzamy kolejne operacje 
                var isAllExist = foundPlaylists.Count == playlistsToUpdate.Count;

                if (isAllExist)
                {

                    // polaczenie w pary wyszukiwanych elemntow ze znalezionymi odpowiednikami w bazie danych, oraz  
                    var margedTwoList = playlistsToUpdate.Zip((foundPlaylists), (toUpdate, founded) => new { ToUpdate = toUpdate, Founded = founded }).ToList();

                    // zamiana "title" w bazie danych na nowe "title" 
                    foreach (var pairData in margedTwoList)
                    {
                        var user = _userRepository.Find(pairData.ToUpdate.UserId);

                        pairData.Founded.Name = pairData.ToUpdate.Name;
                        pairData.Founded.UserId = user.Id;
                        context.Update(pairData.Founded);

                        var playlistToDelete = context.PlaylistsSongs.Where(x => x.PlaylistId == pairData.Founded.Id).ToList();
                        context.PlaylistsSongs.RemoveRange(playlistToDelete);

                        context.SaveChanges();


                        var songsExternalIds = _songRepository.FindMany(pairData.ToUpdate.SongsIds).ToList();

                        var foundSongs = context.Songs.Where(x => songsExternalIds.Contains(x.ExternalId)).ToList();

                        foreach (var song in foundSongs)
                        {
                            context.PlaylistsSongs.Add(new PlaylistSong
                            {
                                SongId = song.Id,
                                PlaylistId = pairData.Founded.Id

                            });
                        }
                        context.SaveChanges();

                    }
                    // zapisanie danych
                    context.SaveChanges();

                    // wyciagniecie ExternalIds edztowanych albumow
                    return foundPlaylists.Select(x => x.ExternalId);

                }
                return null;

            }
            return null;

        }

        public void Delete(Guid playlistExternalId)
        {
            using var context = _factory.CreateDbContext();

            var playlistToDelete = Find(playlistExternalId);

            context.Remove(playlistToDelete);
            context.SaveChanges();
        }

        public void DeleteMany(List<Guid> playlistsExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var playlistsToDelete = context.Playlists.Where(x => playlistsExternalIds.Contains(x.ExternalId)).ToList();

            context.RemoveRange(playlistsToDelete);
            context.SaveChanges();
        }
    }
}
