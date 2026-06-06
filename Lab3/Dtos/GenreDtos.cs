using System.ComponentModel.DataAnnotations;

namespace Lab3.Dtos
{
    public class GenreRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class GenreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MangaCount { get; set; }
    }
}
