using DTO.Models.Artist;
using System.Linq;
using DTO.Models.Category;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Repositories
{
    public class CategoryRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;

        public CategoryRepository(IDbContextFactory<DatabaseContext> factory)
        {
            _factory = factory;
        }

        public Guid Create(string name)
        {
            using var context = _factory.CreateDbContext();

            var externalId = Guid.NewGuid();

            context.Add(new Category 
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

            if (names.Count > 0)
            {
                foreach (var name in names)
                {
                    var externalId = Guid.NewGuid();

                    context.Categories.Add(new Category
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

        public Category Find(Guid categoryExternalId)
        {
            using var context = _factory.CreateDbContext();
            return context.Categories.FirstOrDefault(x => x.ExternalId == categoryExternalId);
        }

        public IEnumerable<Category> FindMany(List<Guid> categoryExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var foundCategory = context.Categories.Where(x => categoryExternalIds.Contains(x.ExternalId)).ToList();

            return foundCategory;
        }


        public Guid Edit(Guid categoryExternalId, UpdateCategoryModel categoryToUpdate)
        {
            using var context = _factory.CreateDbContext();
            var category = Find(categoryExternalId);

            category.Name = categoryToUpdate.Name;

            context.Categories.Update(category);
            context.SaveChanges();

            return category.ExternalId;
        }

        public IEnumerable<Guid> EditMany(IReadOnlyList<UpdateCategoryModel> categoryToUpdate)
        {
            // Sprawdzenie czy w podanej liscie albumow do zmiany nie znajduja sie duplikaty 
            var containsDuplicates = categoryToUpdate.GroupBy(x => x.Name).Select(x => x.Count()).FirstOrDefault(x => x > 1) == null;

            // Jesli duplikaty sie nie znajduja to przeprowadzamy kolejne operacje
            if (!containsDuplicates)
            {
                // zebranie podanych ExternalIds z albums do listy
                var categoriesIds = categoryToUpdate.Select(x => x.ExternalId).ToList();

                using var context = _factory.CreateDbContext();

                // Wyszukanie w bazie danych albumow o podanych ExternalIds i zapisanie ich do listy
                var foundCategories = context.Categories.Where(x => categoriesIds.Contains(x.ExternalId)).ToList();

                // Jesli liczba podanych elementow zgadza sie z liczba wyszukanych w bazie danych albumow - przeprowadzamy kolejne operacje 
                var isAllExist = foundCategories.Count == categoryToUpdate.Count;

                if (isAllExist)
                {
                    // polaczenie w pary wyszukiwanych elemntow ze znalezionymi odpowiednikami w bazie danych, oraz  
                    var margedTwoList = categoryToUpdate.Zip((foundCategories), (toUpdate, founded) => new { ToUpdate = toUpdate, Founded = founded }).ToList();

                    // zamiana "title" w bazie danych na nowe "title" 
                    foreach (var pairData in margedTwoList)
                    {
                        pairData.Founded.Name = pairData.ToUpdate.Name;
                        context.Update(pairData.Founded);
                    }

                    // zapisanie danych
                    context.SaveChanges();

                    // wyciagniecie ExternalIds edztowanych albumow
                    return foundCategories.Select(x => x.ExternalId);
                }
                return null;
            }
            return null;
        }

        public void Delete(Guid categoryExternalId)
        {
            using var context = _factory.CreateDbContext();

            var categoryToDelete = Find(categoryExternalId);

            context.Remove(categoryToDelete);
            context.SaveChanges();
        }

        public void DeleteMany(List<Guid> categoryExternalIds)
        {
            using var context = _factory.CreateDbContext();

            var categoryToDelete = FindMany(categoryExternalIds);

            context.RemoveRange(categoryToDelete);
            context.SaveChanges();
        }


    }
}
