using Lab3.Data;
using Lab3.Dtos;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    [ApiController]
    [Route("api/mangakas")]
    public class MangakasController : ControllerBase
    {
        private readonly MangaDbContext dbContext;

        public MangakasController(MangaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<MangakaResponse>>> GetAll()
        {
            List<MangakaResponse> mangakas = await dbContext.Mangakas
                .OrderBy(mangaka => mangaka.Id)
                .Select(mangaka => new MangakaResponse
                {
                    Id = mangaka.Id,
                    Name = mangaka.Name,
                    Country = mangaka.Country,
                    MangaCount = mangaka.Mangas.Count
                })
                .ToListAsync();

            return Ok(mangakas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MangakaResponse>> GetById(int id)
        {
            MangakaResponse? mangaka = await dbContext.Mangakas
                .Where(item => item.Id == id)
                .Select(item => new MangakaResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    Country = item.Country,
                    MangaCount = item.Mangas.Count
                })
                .FirstOrDefaultAsync();

            if (mangaka == null)
            {
                return NotFound(new { message = "Мангака не найден." });
            }

            return Ok(mangaka);
        }

        [HttpPost]
        public async Task<ActionResult<MangakaResponse>> Create(MangakaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Country))
            {
                return BadRequest(new { message = "Имя и страна обязательны." });
            }

            Mangaka mangaka = new Mangaka
            {
                Name = request.Name.Trim(),
                Country = request.Country.Trim()
            };

            dbContext.Mangakas.Add(mangaka);
            await dbContext.SaveChangesAsync();

            MangakaResponse response = new MangakaResponse
            {
                Id = mangaka.Id,
                Name = mangaka.Name,
                Country = mangaka.Country,
                MangaCount = 0
            };

            return CreatedAtAction(nameof(GetById), new { id = mangaka.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MangakaResponse>> Update(int id, MangakaRequest request)
        {
            Mangaka? mangaka = await dbContext.Mangakas.FindAsync(id);

            if (mangaka == null)
            {
                return NotFound(new { message = "Мангака не найден." });
            }

            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Country))
            {
                return BadRequest(new { message = "Имя и страна обязательны." });
            }

            mangaka.Name = request.Name.Trim();
            mangaka.Country = request.Country.Trim();

            await dbContext.SaveChangesAsync();

            int mangaCount = await dbContext.Mangas.CountAsync(manga => manga.MangakaId == id);

            return Ok(new MangakaResponse
            {
                Id = mangaka.Id,
                Name = mangaka.Name,
                Country = mangaka.Country,
                MangaCount = mangaCount
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Mangaka? mangaka = await dbContext.Mangakas.FindAsync(id);

            if (mangaka == null)
            {
                return NotFound(new { message = "Мангака не найден." });
            }

            bool isUsed = await dbContext.Mangas.AnyAsync(manga => manga.MangakaId == id);

            if (isUsed)
            {
                return Conflict(new { message = "Нельзя удалить мангаку, пока у него есть манга." });
            }

            dbContext.Mangakas.Remove(mangaka);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
