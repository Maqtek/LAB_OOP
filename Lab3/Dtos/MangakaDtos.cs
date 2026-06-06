using System.ComponentModel.DataAnnotations;

namespace Lab3.Dtos
{
    public class MangakaRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;
    }

    public class MangakaResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int MangaCount { get; set; }
    }
}
