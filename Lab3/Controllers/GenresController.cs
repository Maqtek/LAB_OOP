using Lab3.Data;
using Lab3.Dtos;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly MangaDbContext dbContext;

        public GenresController(MangaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreResponse>>> GetAll()
        {
            List<GenreResponse> genres = await dbContext.Genres
                .OrderBy(genre => genre.Id)
                .Select(genre => new GenreResponse
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    MangaCount = genre.Mangas.Count
                })
                .ToListAsync();

            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreResponse>> GetById(int id)
        {
            GenreResponse? genre = await dbContext.Genres
                .Where(item => item.Id == id)
                .Select(item => new GenreResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    MangaCount = item.Mangas.Count
                })
                .FirstOrDefaultAsync();

            if (genre == null)
            {
                return NotFound(new { message = "Жанр не найден." });
            }

            return Ok(genre);
        }

        [HttpPost]
        public async Task<ActionResult<GenreResponse>> Create(GenreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Название жанра обязательно." });
            }

            Genre genre = new Genre
            {
                Name = request.Name.Trim()
            };

            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();

            GenreResponse response = new GenreResponse
            {
                Id = genre.Id,
                Name = genre.Name,
                MangaCount = 0
            };

            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GenreResponse>> Update(int id, GenreRequest request)
        {
            Genre? genre = await dbContext.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound(new { message = "Жанр не найден." });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Название жанра обязательно." });
            }

            genre.Name = request.Name.Trim();
            await dbContext.SaveChangesAsync();

            int mangaCount = await dbContext.Mangas.CountAsync(manga => manga.Genres.Any(item => item.Id == id));

            return Ok(new GenreResponse
            {
                Id = genre.Id,
                Name = genre.Name,
                MangaCount = mangaCount
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Genre? genre = await dbContext.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound(new { message = "Жанр не найден." });
            }

            bool isUsed = await dbContext.Mangas.AnyAsync(manga => manga.Genres.Any(item => item.Id == id));

            if (isUsed)
            {
                return Conflict(new { message = "Нельзя удалить жанр, пока он используется мангой." });
            }

            dbContext.Genres.Remove(genre);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
