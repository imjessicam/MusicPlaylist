using DTO.Models.Album;
using DTO.Models.User;
using Microsoft.AspNetCore.Mvc;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("create")]

        public IActionResult Create([FromBody] CreateUserModel user)
        {
            return Ok(_userRepository.Create(user.Name));
        }

        [HttpPost]
        [Route("createMany")]
        public IActionResult CreateMany([FromBody] CreateManyUserModel user)
        {
            return Ok(_userRepository.CreateMany(user.Names));
        }

        [HttpGet]
        [Route("find")]
        public IActionResult Find(Guid userExternalId)
        {
            var foundUser = _userRepository.Find(userExternalId);
            if (foundUser == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                Name = foundUser.Name
            });

        }

        [HttpGet]
        [Route("findMany")]
        public IActionResult FindMany([FromQuery] List<Guid> usersExternalIds)
        {
            var foundUsers = _userRepository.FindMany(usersExternalIds);
            if (foundUsers.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (foundUsers.Count() != usersExternalIds.Count())
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            if (foundUsers.Count() == usersExternalIds.Count())
            {
                var mappedElements = new List<object>();
                foreach (var user in foundUsers)
                {
                    mappedElements.Add(new { Name = user.Name });
                }
                return Ok(mappedElements);
            }
            return Ok();
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Edit(Guid userExternalId, [FromBody] UpdateUserModel userToUpdate)
        {
            var foundUser = _userRepository.Find(userExternalId);
            if (foundUser == null)
            {
                return NotFound();
            }
            return Ok(_userRepository.Edit(userExternalId, userToUpdate));
        }

        [HttpPut]
        [Route("updateMany")]
        public IActionResult EditMany([FromBody] IReadOnlyList<UpdateUserModel> usersToUpdate)
        {
            // Wybranie z listy podanych usersToUpdate ExternalIds
            var usersExternalIds = usersToUpdate.Select(x => x.ExternalId).ToList();

            // Wyszukanie w bazie danych users z podanym wczesniej ExternalId
            var users = _userRepository.FindMany(usersExternalIds);

            // Jesli liczba znalezionych albumow = 0 - blad!
            if (users.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            // jesli liczba znalezinych w bazie danych albumow zgadza sie z liczba podanych elementow - edytcja.
            return Ok(_userRepository.EditMany(usersToUpdate));
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(Guid userExternalId)
        {
            var foundUser = _userRepository.Find(userExternalId);
            if (foundUser == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            _userRepository.Delete(userExternalId);

            return Ok();
        }

        [HttpDelete]
        [Route("deleteMany")]
        public IActionResult DeleteMany(List<Guid> usersExternalIds)
        {
            var foundUser = _userRepository.FindMany(usersExternalIds);
            if (foundUser.Count() == 0)
            {
                return NotFound();
            }
            if (foundUser.Count() != usersExternalIds.Count())
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            if (foundUser.Count() == usersExternalIds.Count())
            {
                _userRepository.DeleteMany(usersExternalIds);
                return Ok();
            }

            return Ok();
        }
    }
}
