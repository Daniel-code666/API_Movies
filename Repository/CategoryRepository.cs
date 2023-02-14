using API_Movies.Data;
using API_Movies.Models;
using API_Movies.Repository.IRepository;
using System;

namespace API_Movies.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;    
        }

        public bool CreateCategory(Category category)
        {
            category.created_at = DateTime.Now;
            _db.Category.Add(category);
            return Save();
        }

        public bool CategoryExist(string name)
        {
            bool value = _db.Category.Any(c => c.name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool CategoryExists(int id)
        {
            bool value = _db.Category.Any(c => c.id == id);
            return value;
        }

        public bool DeleteCategory(int id)
        {
            var cat = _db.Category.FirstOrDefault(c => c.id == id);
            _db.Category.Remove(cat);
            return Save();
        }

        public ICollection<Category> GetAllCategory()
        {
            return _db.Category.ToList();
        }

        public Category GetSingleCategory(int id)
        {
            return _db.Category.FirstOrDefault(c => c.id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdtCategory(Category category)
        {
            _db.ChangeTracker.Clear();
            category.created_at = DateTime.Now;
            _db.Category.Update(category);
            return Save();
        }
    }
}
