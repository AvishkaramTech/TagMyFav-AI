using Avish.Core.Interfaces;
using Avish.Core.Model;
using TagMyFav_AI.Interfaces;

namespace TagMyFav_AI.Helpers
{
    internal class CategoryHelper:ICategoryHelper
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryHelper(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> CreateDefaultCategory(int userID)
        {
            var added = await _categoryRepository.Add(new Category
            {
                Name = "Unsorted",
                Description = "Created By Default",
                IsShared = false,
                IsPrivate = true,
                IsPublic = false,
                IsActive = true,
                CreatedBy = userID,
                ModifiedBy = userID
            });
            return Convert.ToInt32(added.ID);
        }

        public async Task<int> CheckForCategory(int userID)
        {
            int catID = 0;
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@createdby", userID);

            var category = await _categoryRepository.GetAll(parameters);
            if (category == null)
            {
                catID = await CreateDefaultCategory(userID);
            }
            else if (category.Count() == 0)
            {
                catID = await CreateDefaultCategory(userID);
            }
            return catID;
        }
    }
}
