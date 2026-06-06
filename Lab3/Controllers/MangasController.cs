using Lab3.Data;
using Lab3.Dtos;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    [ApiController]
    [Route("api/mangas")]
    public class MangasController : ControllerBase
    {
        private readonly MangaDbContext dbContext;

        public MangasController(MangaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<MangaResponse>>> GetAll()
        {
            List<Manga> mangas = await dbContext.Mangas
                .Include(manga => manga.Mangaka)
                .Include(manga => manga.Genres)
                .OrderBy(manga => manga.Id)
                .ToListAsync();

            List<MangaResponse> responses = new List<MangaResponse>();

            for (int i = 0; i < mangas.Count; i++)
            {
                responses.Add(ResponseMapper.ToResponse(mangas[i]));
            }

            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MangaResponse>> GetById(int id)
        {
            Manga? manga = await FindManga(id);

            if (manga == null)
            {
                return NotFound(new { message = "Манга не найдена." });
            }

            return Ok(ResponseMapper.ToResponse(manga));
        }

        [HttpPost]
        public async Task<ActionResult<MangaResponse>> Create(MangaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Название манги обязательно." });
            }

            Mangaka? mangaka = await dbContext.Mangakas.FindAsync(request.MangakaId);

            if (mangaka == null)
            {
                return BadRequest(new { message = "Указанный мангака не существует." });
            }

            List<Genre>? genres = await FindGenres(request.GenreIds);

            if (genres == null)
            {
                return BadRequest(new { message = "Один или несколько указанных жанров не существуют." });
            }

            Manga manga = new Manga
            {
                Title = request.Title.Trim(),
                ReleaseYear = request.ReleaseYear,
                MangakaId = mangaka.Id,
                Mangaka = mangaka,
                Genres = genres
            };

            dbContext.Mangas.Add(manga);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = manga.Id }, ResponseMapper.ToResponse(manga));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MangaResponse>> Update(int id, MangaRequest request)
        {
            Manga? manga = await FindManga(id);

            if (manga == null)
            {
                return NotFound(new { message = "Манга не найдена." });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Название манги обязательно." });
            }

            Mangaka? mangaka = await dbContext.Mangakas.FindAsync(request.MangakaId);

            if (mangaka == null)
            {
                return BadRequest(new { message = "Указанный мангака не существует." });
            }

            List<Genre>? genres = await FindGenres(request.GenreIds);

            if (genres == null)
            {
                return BadRequest(new { message = "Один или несколько указанных жанров не существуют." });
            }

            manga.Title = request.Title.Trim();
            manga.ReleaseYear = request.ReleaseYear;
            manga.MangakaId = mangaka.Id;
            manga.Mangaka = mangaka;
            manga.Genres.Clear();

            for (int i = 0; i < genres.Count; i++)
            {
                manga.Genres.Add(genres[i]);
            }

            await dbContext.SaveChangesAsync();

            return Ok(ResponseMapper.ToResponse(manga));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Manga? manga = await dbContext.Mangas.FindAsync(id);

            if (manga == null)
            {
                return NotFound(new { message = "Манга не найдена." });
            }

            dbContext.Mangas.Remove(manga);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private async Task<Manga?> FindManga(int id)
        {
            return await dbContext.Mangas
                .Include(manga => manga.Mangaka)
                .Include(manga => manga.Genres)
                .FirstOrDefaultAsync(manga => manga.Id == id);
        }

        private async Task<List<Genre>?> FindGenres(List<int> requestedGenreIds)
        {
            List<int> genreIds = requestedGenreIds.Distinct().ToList();
            List<Genre> genres = await dbContext.Genres
                .Where(genre => genreIds.Contains(genre.Id))
                .ToListAsync();

            if (genres.Count != genreIds.Count)
            {
                return null;
            }

            return genres;
        }
    }
}
