using DTO.Models.Album;
using DTO.Models.Category;
using Microsoft.AspNetCore.Mvc;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement.Controllers
{
    [ApiController]
    [Route("Category")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Route("create")]

        public IActionResult Create([FromBody] CreateCategoryModel category)
        {
            return Ok(_categoryRepository.Create(category.Name));
        }

        [HttpPost]
        [Route("createMany")]
        public IActionResult CreateMany([FromBody] CreateManyCategoryModel category)
        {
            return Ok(_categoryRepository.CreateMany(category.Names));
        }

        [HttpGet]
        [Route("find")]
        public IActionResult Find(Guid categoryExternalId)
        {
            var foundCategory = _categoryRepository.Find(categoryExternalId);
            if (foundCategory == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                Name = foundCategory.Name
            });

        }

        [HttpGet]
        [Route("findMany")]
        public IActionResult FindMany([FromQuery] List<Guid> categoriesExternalIds)
        {
            var foundCategories = _categoryRepository.FindMany(categoriesExternalIds);
            if (foundCategories.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (foundCategories.Count() != categoriesExternalIds.Count())
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            if (foundCategories.Count() == categoriesExternalIds.Count())
            {
                var mappedElements = new List<object>();
                foreach (var category in foundCategories)
                {
                    mappedElements.Add(new { Name = category.Name });
                }
                return Ok(mappedElements);
            }
            return Ok();
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Edit(Guid categoryExternalId, [FromBody] UpdateCategoryModel categoryToUpdate)
        {
            var foundCategory = _categoryRepository.Find(categoryExternalId);
            if (foundCategory == null)
            {
                return NotFound();
            }
            return Ok(_categoryRepository.Edit(categoryExternalId, categoryToUpdate));

        }

        [HttpPut]
        [Route("updateMany")]
        public IActionResult EditMany([FromBody] IReadOnlyList<UpdateCategoryModel> categoriesToUpdate)
        {
            // Wybranie z listy podanych albumsToUpdate ExternalIds
            var categoriesExternalIds = categoriesToUpdate.Select(x => x.ExternalId).ToList();

            // Wyszukanie w bazie danych albumow z podanym wczesniej ExternalId
            var categories = _categoryRepository.FindMany(categoriesExternalIds);

            // Jesli liczba znalezionych albumow = 0 - blad!
            if (categories.Count() == 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            // jesli liczba znalezinych w bazie danych albumow zgadza sie z liczba podanych elementow - edytcja.
            return Ok(_categoryRepository.EditMany(categoriesToUpdate));
        }


        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(Guid categoryExternalId)
        {
            var foundCategory = _categoryRepository.Find(categoryExternalId);
            if (foundCategory == null)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            _categoryRepository.Delete(categoryExternalId);

            return Ok();
        }

        [HttpDelete]
        [Route("deleteMany")]
        public IActionResult DeleteMany(List<Guid> categoriesExternalIds)
        {
            var foundCategories = _categoryRepository.FindMany(categoriesExternalIds);
            if (foundCategories.Count() == 0)
            {
                return NotFound();
            }
            if (foundCategories.Count() != categoriesExternalIds.Count())
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            if (foundCategories.Count() == categoriesExternalIds.Count())
            {
                _categoryRepository.DeleteMany(categoriesExternalIds);
                return Ok();
            }
            return Ok();
        }
    }
}
