using API_Movies.Models;
using API_Movies.Models.Dtos;

namespace API_Movies.Repository.IRepository
{
    public interface IMovieRepository
    {
        ICollection<Movie> GetAllMovies();

        object GetSingleMovie(int id);

        bool CreateMovie(Movie movie);

        bool MovieExist(string title);

        bool MovieExists(int id);

        bool UpdtMovie(Movie movie);

        bool DeleteMovie(int id);

        ICollection<Movie> GetMovieInCategory(int catId);

        ICollection<Movie> SearchMovie(string title);

        bool Save();
    }
}
