using Lab3.Models;

namespace Lab3.Dtos
{
    internal static class ResponseMapper
    {
        public static MangaResponse ToResponse(Manga manga)
        {
            MangaResponse response = new MangaResponse
            {
                Id = manga.Id,
                Title = manga.Title,
                ReleaseYear = manga.ReleaseYear,
                Mangaka = new MangaMangakaResponse
                {
                    Id = manga.Mangaka.Id,
                    Name = manga.Mangaka.Name,
                    Country = manga.Mangaka.Country
                }
            };

            for (int i = 0; i < manga.Genres.Count; i++)
            {
                Genre genre = manga.Genres[i];

                response.Genres.Add(new MangaGenreResponse
                {
                    Id = genre.Id,
                    Name = genre.Name
                });
            }

            return response;
        }
    }
}
