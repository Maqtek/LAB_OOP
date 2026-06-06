using System.ComponentModel.DataAnnotations;

namespace Lab3.Dtos
{
    public class MangaRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int ReleaseYear { get; set; }

        [Range(1, int.MaxValue)]
        public int MangakaId { get; set; }

        public List<int> GenreIds { get; set; } = new List<int>();
    }

    public class MangaResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public MangaMangakaResponse Mangaka { get; set; } = new MangaMangakaResponse();
        public List<MangaGenreResponse> Genres { get; set; } = new List<MangaGenreResponse>();
    }

    public class MangaMangakaResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class MangaGenreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
