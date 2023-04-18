using DTO.Models.Album;
using DTO.Models.Artist;
using Microsoft.AspNetCore.Mvc;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement.Controllers
{
    [ApiController]
    [Route("Artist")]
    public class ArtistController : ControllerBase
    {
        private readonly ArtistRepository _artistRepository;

        public ArtistController(ArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        [HttpPost]
        [Route("create")]

        public IActionResult Create([FromBody] CreateArtistModel artist)
        {
            return Ok(_artistRepository.Create(artist.Name));
        }

        [HttpPost]
        [Route("createMany")]
        public IActionResult CreateMany([FromBody] CreateManyArtistModel artists)
        {
            return Ok(_artistRepository.CreateMany(artists.Names));
        }

        [HttpGet]
        [Route("find")]
        public IActionResult Find(Guid artistExternalId)
        {
            var foundArtist = _artistRepository.Find(artistExternalId);
            if(foundArtist == null)
            {
                return NotFound();
            }
            return Ok(new 
            {
                Name = foundArtist.Name 
            });

        }

        [HttpGet]
        [Route("findMany")]
        public IActionResult FindMany([FromQuery] List<Guid> artistsExternalIds)
        {
            var foundArtists = _artistRepository.FindMany(artistsExternalIds);
            if (foundArtists.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (foundArtists.Count() != artistsExternalIds.Count())
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            if(foundArtists.Count() == artistsExternalIds.Count())
            {
                var mappedElements = new List<object>();
                foreach(var artist in foundArtists)
                {
                    mappedElements.Add(new { Name = artist.Name});
                }
                
                return Ok(mappedElements);
            }

            return Ok();
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Edit(Guid artistExternalId, [FromBody] UpdateArtistModel artistToUpdate)
        {
            var foundArtist = _artistRepository.Find(artistExternalId);
            if(foundArtist == null)
            {
                return NotFound();
            }
            return Ok(_artistRepository.Edit(artistExternalId, artistToUpdate));

        }

        [HttpPut]
        [Route("updateMany")]
        public IActionResult EditMany([FromBody] IReadOnlyList<UpdateArtistModel> artistsToUpdate)
        {
            // Wybranie z listy podanych albumsToUpdate ExternalIds
            var artistsExternalIds = artistsToUpdate.Select(x => x.ExternalId).ToList();

            // Wyszukanie w bazie danych albumow z podanym wczesniej ExternalId
            var artists = _artistRepository.FindMany(artistsExternalIds);

            // Jesli liczba znalezionych albumow = 0 - blad!
            if (artists.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            // jesli liczba znalezinych w bazie danych albumow zgadza sie z liczba podanych elementow - edytcja.
            return Ok(_artistRepository.EditMany(artistsToUpdate));
        }


        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(Guid artistExternalId)
        {
            var foundArtist = _artistRepository.Find(artistExternalId);
            if(foundArtist == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            _artistRepository.Delete(artistExternalId);

            return Ok();
        }

        [HttpDelete]
        [Route("deleteMany")]
        public IActionResult DeleteMany(List<Guid> artistsExternalIds)
        {
            var foundArtists = _artistRepository.FindMany(artistsExternalIds);
            if(foundArtists.Count() == 0)
            {
                return NotFound();
            }
            if(foundArtists.Count() != artistsExternalIds.Count())
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            if(foundArtists.Count() == artistsExternalIds.Count())
            {
                _artistRepository.DeleteMany(artistsExternalIds);
                return Ok();
            }

            return Ok();
        }
    }
}
