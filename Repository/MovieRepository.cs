using API_Movies.Data;
using API_Movies.Models;
using API_Movies.Models.Dtos;
using API_Movies.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API_Movies.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _db;

        public MovieRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateMovie(Movie movie)
        {
            if(movie.imgRoute == null)
            {
                movie.imgRoute = "/";
            }

            movie.movie_created_at = DateTime.Now;
            _db.Movie.Add(movie);
            return Save();
        }

        public bool DeleteMovie(int id)
        {
            var movie = _db.Movie.FirstOrDefault(m => m.idMovie == id);
            _db.Movie.Remove(movie);
            return Save();
        }

        public ICollection<Movie> GetAllMovies()
        {
            List<Movie> moviesList = new();

            var movResult = (from m in _db.Movie join c in _db.Category on m.categoryId equals c.id select new
            {
                m.idMovie, m.title, m.description, m.imgRoute, m.duration, m.rating, m.movie_created_at, m.categoryId,
                c.id, c.name, c.created_at
            }).ToList();
            
            /*
            var movResult = _db.Movie.Join(_db.Category, m => m.categoryId, c => c.id, (movie, category) => new {
                movie, category
            });

            foreach(var item in movResult)
            {
                var movie = new Movie
                {
                    idMovie = item.movie.idMovie,
                    category = new Category
                    {
                        id = item.category.id
                    }
                }
            }
             */

            foreach(var item in movResult)
            {
                var movie = new Movie
                {
                    idMovie = item.idMovie,
                    description = item.description,
                    title = item.title,
                    imgRoute = item.imgRoute,
                    duration = item.duration,
                    rating = item.rating,
                    movie_created_at = item.movie_created_at,
                    categoryId = item.categoryId,
                    category = new Category
                    {
                        id = item.categoryId,
                        name = item.name,
                        created_at = item.created_at
                    }
                };

                moviesList.Add(movie);
            }

            return moviesList;
        }

        public ICollection<Movie> GetMovieInCategory(int catId)
        {
            return _db.Movie.Include(m => m.category).Where(m => m.categoryId == catId).ToList();
        }

        public object GetSingleMovie(int id)
        {
            var movResult = (from m in _db.Movie
                          join c in _db.Category on m.categoryId equals c.id
                          select new
                          {
                              m.idMovie,
                              m.title,
                              m.description,
                              m.imgRoute,
                              m.duration,
                              m.rating,
                              m.movie_created_at,
                              m.categoryId,
                              c.id,
                              c.name,
                              c.created_at
                          }).Where(m => m.idMovie == id).FirstOrDefault();

            var movie = new Movie()
            {
                idMovie = movResult.idMovie,
                title = movResult.title,
                description = movResult.description,
                imgRoute = movResult.imgRoute,
                duration = movResult.duration,
                rating = movResult.rating,
                movie_created_at = movResult.movie_created_at,
                categoryId = movResult.categoryId,
                category = new Category()
                {
                    id = movResult.id,
                    name = movResult.name,
                    created_at = movResult.created_at
                } 
            };

            return movie;
        }

        public bool MovieExist(string title)
        {
            return _db.Movie.Any(m => m.title == title);
        }

        public bool MovieExists(int id)
        {
            return _db.Movie.Any(m => m.idMovie == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public ICollection<Movie> SearchMovie(string searchText)
        {
            // IQueryable<Movie> query = _db.Movie;

            // query = query.Where(m => m.title.Contains(searchText) || m.description.Contains(searchText));

            List<Movie> moviesList = new();

            var movResult = (from m in _db.Movie
                             join c in _db.Category on m.categoryId equals c.id
                             select new
                             {
                                 m.idMovie,
                                 m.title,
                                 m.description,
                                 m.imgRoute,
                                 m.duration,
                                 m.rating,
                                 m.movie_created_at,
                                 m.categoryId,
                                 c.id,
                                 c.name,
                                 c.created_at
                             }).Where(m => m.title.Contains(searchText) || m.description.Contains(searchText)).ToList();

            foreach (var item in movResult)
            {
                var movie = new Movie
                {
                    idMovie = item.idMovie,
                    description = item.description,
                    title = item.title,
                    imgRoute = item.imgRoute,
                    duration = item.duration,
                    rating = item.rating,
                    movie_created_at = item.movie_created_at,
                    categoryId = item.categoryId,
                    category = new Category
                    {
                        id = item.categoryId,
                        name = item.name,
                        created_at = item.created_at
                    }
                };

                moviesList.Add(movie);
            }

            return moviesList;
        }

        public bool UpdtMovie(Movie movie)
        {
            if (movie.imgRoute == null)
            {
                movie.imgRoute = "/";
            }

            movie.movie_created_at = DateTime.Now;
            _db.Movie.Update(movie);
            return Save();
        }
    }
}
