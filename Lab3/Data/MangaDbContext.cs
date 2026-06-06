using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Data
{
    public class MangaDbContext : DbContext
    {
        public MangaDbContext(DbContextOptions<MangaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mangaka> Mangakas => Set<Mangaka>();
        public DbSet<Manga> Mangas => Set<Manga>();
        public DbSet<Genre> Genres => Set<Genre>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mangaka>()
                .HasMany(mangaka => mangaka.Mangas)
                .WithOne(manga => manga.Mangaka)
                .HasForeignKey(manga => manga.MangakaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Manga>()
                .HasMany(manga => manga.Genres)
                .WithMany(genre => genre.Mangas)
                .UsingEntity("MangaGenres");

            modelBuilder.Entity<Mangaka>()
                .Property(mangaka => mangaka.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Mangaka>()
                .Property(mangaka => mangaka.Country)
                .HasMaxLength(100);

            modelBuilder.Entity<Manga>()
                .Property(manga => manga.Title)
                .HasMaxLength(200);

            modelBuilder.Entity<Genre>()
                .Property(genre => genre.Name)
                .HasMaxLength(100);
        }
    }
}
