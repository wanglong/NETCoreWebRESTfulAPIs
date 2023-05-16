using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCoreWebRESTfulAPIs.EF;
using NETCoreWebRESTfulAPIs.Models;

namespace NETCoreWebRESTfulAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _movieContext;

        public MoviesController(MovieContext movieContext)
        {
            _movieContext = movieContext;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if(_movieContext == null)
            {
                return NotFound();
            }

            return await _movieContext.Movies.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            if (_movieContext == null)
            {
                return NotFound();
            }

            var movie = await _movieContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            await _movieContext.AddAsync(movie);
            await _movieContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id}, movie);
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if(id != movie.Id)
            {
                return BadRequest();
            }

            _movieContext.Entry(movie).State = EntityState.Modified;

            try
            {
                await _movieContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) 
            {
                if(!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (_movieContext.Movies == null)
            {
                return NotFound();
            }

            var movie = await _movieContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _movieContext.Movies.Remove(movie);
            await _movieContext.SaveChangesAsync();

            return NoContent();
        }



        private bool MovieExists(long id)
        {
            return (_movieContext.Movies?.Any(x => x.Id == id)).GetValueOrDefault();
        }
    }
}
