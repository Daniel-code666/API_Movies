using API_Movies.Models;

namespace API_Movies.Repository.IRepository
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetAllCategory();

        Category GetSingleCategory(int id);

        bool CreateCategory(Category category);

        bool CategoryExist(string name);

        bool CategoryExists(int id);

        bool UpdtCategory(Category category);

        bool DeleteCategory(int id);

        bool Save();
    }
}
