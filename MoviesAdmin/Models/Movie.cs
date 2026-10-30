using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        [Required]
        public string Summary { get; set; } = string.Empty;

        [Display(Name = "RunTime (min)")]
        [Required]
        public double RunTime { get; set; } //126 minutes or 2h 6m

        [ValidateNever]
        public List<CriticReview> CriticReviews { get; set;} = new List<CriticReview>();

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [ValidateNever]
        public Genre Genre { get; set; } = null!;

        [Display(Name = "Audience Rating")]
        public int AudienceRatingId { get; set; }

        [ValidateNever]
        public AudienceRating AudienceRating { get; set; } = null!;

        [Required]
        public DateTime ReleaseDate { get; set; } //e.g August 24,2026

        public string? ImageUrl { get; set; } = string.Empty; //poster of the movie e.g matrix.png
    }
}
