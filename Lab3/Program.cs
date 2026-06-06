using Lab3.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<MangaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MangaDatabase")));

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    MangaDbContext dbContext = scope.ServiceProvider.GetRequiredService<MangaDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapControllers();
app.Run();
