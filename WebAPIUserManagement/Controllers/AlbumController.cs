using DTO.Models.Album;
using Microsoft.AspNetCore.Mvc;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement.Controllers
{
    [ApiController]
    [Route("album")]
    public class AlbumController : ControllerBase
    {
        private readonly AlbumRepository _albumRepository;

        public AlbumController(AlbumRepository addressRepository)
        {
            _albumRepository = addressRepository;
        }

        [HttpPost]
        [Route("create")]

        public IActionResult Create([FromBody] CreateAlbumModel album)
        {
            var newAlbum = new string (album.Title);
            if(newAlbum.Count() == 0)
            {
                return BadRequest();
            }
            return Ok(_albumRepository.Create(album.Title));

        }

        [HttpPost]
        [Route("createMany")]

        public IActionResult CreateMany([FromBody] CreateManyAlbumModel albums)
        {
            return Ok(_albumRepository.CreateMany(albums.Titles));
        }



        [HttpGet]
        [Route("find")]
        
        public IActionResult Find(Guid albumExternalId)
        {
            var foundAlbum = _albumRepository.Find(albumExternalId);
            if(foundAlbum == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(new 
            {
                Title = foundAlbum.Title 
            });
        }

        [HttpGet]
        [Route("findMany")]

        public IActionResult FindMany([FromQuery] List<Guid> albumsExternalIds)
        {
            var foundAlbums = _albumRepository.FindMany(albumsExternalIds);
            if(foundAlbums.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var titles = new List<object>();            
            
            foreach(var album in foundAlbums)
            {
                titles.Add(new { Title = album.Title});
            };

            return Ok(titles);
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Edit(Guid albumExternalId, [FromBody] UpdateAlbumModel albumToUpdate)
        {
            // find Album
            var foundAlbum = _albumRepository.Find(albumExternalId);

            // check if Album was found
            if(foundAlbum == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            // return 
            return Ok(_albumRepository.Edit(albumExternalId, albumToUpdate));

        }

        [HttpPut]
        [Route("updateMany")]
        public IActionResult EditMany([FromBody] IReadOnlyList<UpdateAlbumModel> albumsToUpdate)
        {
            // Wybranie z listy podanych albumsToUpdate ExternalIds
            var albumsExternalIds = albumsToUpdate.Select(x => x.ExternalId).ToList();

            // Wyszukanie w bazie danych albumow z podanym wczesniej ExternalId
            var albums = _albumRepository.FindMany(albumsExternalIds);  

            // Jesli liczba znalezionych albumow = 0 - blad!
            if(albums.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            // jesli liczba znalezinych w bazie danych albumow zgadza sie z liczba podanych elementow - edytcja.
            return Ok(_albumRepository.EditMany(albumsToUpdate));
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(Guid albumExternalId)
        {
            var albumToDelete = _albumRepository.Find(albumExternalId);
            if(albumToDelete == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            
            _albumRepository.Delete(albumExternalId);
            
            return Ok();
        }

        [HttpDelete]
        [Route("deleteMany")]

        public IActionResult DeleteMany(List<Guid> albumsExternalIds)
        {
            var albumsToDelete = _albumRepository.FindMany(albumsExternalIds);
            if(albumsToDelete.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            _albumRepository.DeleteMany(albumsExternalIds);            

            return Ok();
        }


    }
}
