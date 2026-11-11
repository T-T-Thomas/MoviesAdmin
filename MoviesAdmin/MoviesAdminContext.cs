using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

public class MoviesAdminContext(DbContextOptions<MoviesAdminContext> options) : DbContext(options)
{
    public DbSet<MoviesAdmin.Models.Movie> Movie { get; set; } = default!;

    public DbSet<Genre> Genres { get; set; } = default!;

    public DbSet<AudienceRating> AudienceRatings { get; set; } = default!;

    public DbSet<MoviesAdmin.Models.CriticReview> CriticReview { get; set; } = default!;

    public DbSet<UserSnapshot> UserSnapshot { get; set; } = default!;

    //only allow one review per user per movie
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enforce that a combination of MovieId and CreatedBy must be unique
        modelBuilder.Entity<CriticReview>()
            .HasIndex(r => new { r.MovieId, r.CreatedBy })
            .IsUnique();
    }
}
