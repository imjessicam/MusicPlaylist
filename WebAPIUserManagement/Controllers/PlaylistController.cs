using DTO.Models.Playlist;
using DTO.Models.Song;
using Microsoft.AspNetCore.Mvc;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement.Controllers
{
    [ApiController]
    [Route("Playlist")]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistRepository _playlistRepository;
        private readonly UserRepository _userRepository;
        private readonly SongRepository _songRepository;

        public PlaylistController(PlaylistRepository playlistRepository, UserRepository userRepository, SongRepository songRepository)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _songRepository = songRepository;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([FromBody] CreatePlaylistModel playlist)
        {
            return Ok(_playlistRepository.Create(playlist.Name, playlist.UserId, playlist.SongsIds));
        }

        [HttpPost]
        [Route("createMany")]
        public IActionResult CreateMany([FromBody] IReadOnlyList<CreatePlaylistModel> playlistsToAdd)
        {
            return Ok(_playlistRepository.CreateMany(playlistsToAdd));
        }
       

        [HttpGet]
        [Route("find")]
        public IActionResult Find(Guid playListExternalId)
        {
            var playlist = _playlistRepository.Find(playListExternalId);
            var user = _playlistRepository.GetUser(playListExternalId);
            var songs = _playlistRepository.GetSongs(playListExternalId);

            if(playlist == null)
            {
                return NotFound();
            }
            if(user == null)
            {
                return NotFound();
            }
            if(songs.Count == 0)
            {
                return NotFound();
            }

            var mappedElements = new List<object>();
            foreach(var song in songs)
            {
                mappedElements.Add(new 
                {
                    ExternalId = song.ExternalId, 
                    Title = song.Title
                });
            }

            return Ok(new 
            {
                PlaylistTitle = playlist.Name,
                PlaylistId = playlist.ExternalId,
                UserName = user.Name,
                UserId = user.ExternalId,
                Songs = mappedElements
            });
        }

        [HttpGet]
        [Route("findMany")]
        public IActionResult FindMany([FromQuery] List<Guid> playlistsExternalIds)
        {
            // Znajdz piosenki za pomoca podanych ExternalIds
            var foundPlaylists = _playlistRepository.FindMany(playlistsExternalIds);
            if (foundPlaylists.Count() == 0)
            {
                return NotFound();
            }

            // Stworz listy Songs/Album/Category/Artists dla kazdego songExternalId ze znalezionach songs
            var playlistMappedElements = new List<object>();
            var userMappedElements = new List<object>();
            var songMappedElements = new List<object>();

            foreach (var playlistExternalId in playlistsExternalIds)
            {
                // playlist
                var playlist = _playlistRepository.Find(playlistExternalId);
                playlistMappedElements.Add(new { Name = playlist.Name });

                // user
                var user = _playlistRepository.GetUser(playlistExternalId);
                userMappedElements.Add(new { Name = user.Name });

                // songs
                var songs = _playlistRepository.GetSongs(playlistExternalId);
                foreach (var song in songs)
                {
                    songMappedElements.Add(new { Title = song.Title });
                }

                // [playlist1] - [user][[song][song][song]]
                // [playlist2] - [user][[song]]
                // [playlist3] - [user][[song][song]]
            }

            return Ok(new
            {
                PlaylistName = playlistMappedElements,
                UserName = userMappedElements,
                SongTitle = songMappedElements,
            });

        }

        [HttpPut]
        [Route("update")]
        public IActionResult Edit(Guid playlistExternalId, [FromBody] UpdatePlaylistModel playlistToUpdate)
        {
            var foundPlaylist = _playlistRepository.Find(playlistExternalId);
            if (foundPlaylist == null)
            {
                return NotFound();
            }

            _playlistRepository.Edit(playlistExternalId, playlistToUpdate, playlistToUpdate.SongsIds);
            return Ok(playlistExternalId);
        }

        [HttpPut]
        [Route("updateMany")]
        public IActionResult EditMany([FromBody] IReadOnlyList<UpdatePlaylistModel> playlistsToUpdate)
        {
            // Wybranie z listy podanych songsToUpdate ExternalIds
            var playlistsExternalIds = playlistsToUpdate.Select(x => x.ExternalId).ToList();

            // Wyszukanie w bazie danych songs z podanym wczesniej ExternalId
            var playlist = _playlistRepository.FindMany(playlistsExternalIds);

            // Jesli liczba znalezionych albumow = 0 - blad
            if (playlist.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            return Ok(_playlistRepository.EditMany(playlistsToUpdate));
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(Guid playlistExternalId)
        {
            var playlistToDelete = _playlistRepository.Find(playlistExternalId);
            if(playlistToDelete == null)
            {
                return NotFound();
            }
            _playlistRepository.Delete(playlistExternalId);
            return Ok();
        }

        [HttpDelete]
        [Route("deleteMany")]
        public IActionResult DeleteMany([FromQuery] List<Guid> playlistsExternalIds)
        {
            var playlistsToDelete = _playlistRepository.FindMany(playlistsExternalIds);
            if (playlistsToDelete.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (playlistsToDelete.Count() == playlistsExternalIds.Count)
            {
                _playlistRepository.DeleteMany(playlistsExternalIds);
            }
            return Ok();
        }
    }
}
