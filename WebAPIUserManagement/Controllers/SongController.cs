using DTO.Models.Song;
using Microsoft.AspNetCore.Mvc;
using MusicPlaylistModel.Models;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement.Controllers
{
    [ApiController]
    [Route("Song")]
    public class SongController : ControllerBase
    {
        private readonly SongRepository _songRepository;
        private readonly AlbumRepository _albumRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ArtistRepository _artistRepository;

        public SongController(SongRepository songRepository, AlbumRepository albumRepository, CategoryRepository categoryRepository, ArtistRepository artistRepository)
        {
            _songRepository = songRepository;
            _albumRepository = albumRepository;
            _categoryRepository = categoryRepository;
            _artistRepository = artistRepository;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([FromQuery] CreateSongModel song)
        {
            return Ok(_songRepository.Create(song.Title, song.AlbumId, song.CategoryId, song.ArtistsIds));
        }

        [HttpPost]
        [Route("createMany/oneAlbum/oneCategory")]
        public IActionResult CreateManyInOneAlbum([FromBody] CreateManyInOneAlbumSongModel songs)
        {
            return Ok(_songRepository.CreateManyInOneAlbum(songs.Titles, songs.AlbumId, songs.CategoryId, songs.ArtistsIds));
        }

        // create many different titles, diferent category...
        [HttpPost]
        [Route("createMany")]
        public IActionResult CreateMany([FromBody] IReadOnlyList<CreateSongModel> songsToAdd)
        {
            return Ok(_songRepository.CreateMany(songsToAdd));
        }

        [HttpGet]
        [Route("find")]
        public IActionResult Find(Guid songExternalId)
        {
            var song = _songRepository.Find(songExternalId);

            var album = _songRepository.GetAlbum(songExternalId);
            var category = _songRepository.GetCategory(songExternalId);
            var artists = _songRepository.GetArtists(songExternalId);

            if(song == null)
            {
                return NotFound();
            }
            if(album == null)
            {
                return NotFound();
            }
            if(category == null)
            {
                return NotFound();
            }
            if(artists.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var artistsMappedElements = new List<object>();
            foreach (var artist in artists)
            {
                artistsMappedElements.Add(new { ArtistName = artist.Name });
            }

            return Ok(new
            {
                Title = song.Title,
                Album = album.Title,
                Category = category.Name,
                Artists = artistsMappedElements
            });

        }

        [HttpGet]
        [Route("findMany")]
        public IActionResult FindMany([FromQuery] List<Guid> songsExternalIds)
        {
            // Znajdz piosenki za pomoca podanych ExternalIds
            var foundSongsExternalIds = _songRepository.FindMany(songsExternalIds);
            if (foundSongsExternalIds.Count() == 0)
            {
                return NotFound();
            }

            // utworz liste tytulow piosenek

            var songs = new List<object>();
            var songsDetails = new List<object>();

            foreach (var songExternalId in foundSongsExternalIds)
            {
                var songTitle = _songRepository.Find(songExternalId);
                songs.Add(new { SongTitle = songTitle.Title });

                // utworz list details: album, category, artists
                // - Album
                var album = _songRepository.GetAlbum(songExternalId);
                songs.Add(new { AlbumTitle = album.Title });

                // - Category
                var category = _songRepository.GetCategory(songExternalId);
                songs.Add(new { CategoryName = category.Name });

                // - Artists
                var artists = _songRepository.GetArtists(songExternalId);
                foreach (var artist in artists)
                {
                    songs.Add(new { ArtistName = artist.Name });
                }         
                
            }
            return Ok(songs);

        }

        [HttpPut]
        [Route("update")]
        public IActionResult Edit(Guid songExternalId, [FromBody] UpdateSongModel songToUpdate)
        {
            var foundSong = _songRepository.Find(songExternalId);
            if(foundSong == null)
            {
                return NotFound();
            }

            _songRepository.Edit(songExternalId, songToUpdate, songToUpdate.ArtistsIds);
            return Ok(songExternalId);
        }

        [HttpPut]
        [Route("updateMany")]
        public IActionResult EditMany([FromBody] IReadOnlyList<UpdateSongModel> songsToUpdate)
        {
            // Wybranie z listy podanych songsToUpdate ExternalIds
            var songsExternalIds = songsToUpdate.Select(x => x.ExternalId).ToList();

            // Wyszukanie w bazie danych songs z podanym wczesniej ExternalId
            var songs = _songRepository.FindMany(songsExternalIds);

            // Jesli liczba znalezionych albumow = 0 - blad
            if(songs.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            return Ok(_songRepository.EditMany(songsToUpdate));
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete([FromQuery] Guid songExternalId)
        {
            var songToDelete = _songRepository.Find(songExternalId);
            if(songToDelete == null)
            {
                return NotFound();
            }

            _songRepository.Delete(songExternalId);
            return Ok();
        }

        [HttpDelete]
        [Route("deleteMany")]
        public IActionResult DeleteMany([FromQuery] List<Guid> songsExternalIds)
        {
            var songsToDelete = _songRepository.FindMany(songsExternalIds);
            if (songsToDelete.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (songsToDelete.Count() == songsExternalIds.Count)
            {
                _songRepository.DeleteMany(songsExternalIds);
            }
            return Ok();
        }
    }
}
