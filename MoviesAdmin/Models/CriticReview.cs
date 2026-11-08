using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        public int Id { get; set; }

        [Display(Name = "Movie")]
        public int MovieId {  get; set; }

        [ValidateNever]
        public Movie Movie { get; set; } = null!;

        [StringLength(1000)]
        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating (1-5 stars)")]
        public float Rating { get; set; }

        [Required]
        [Display(Name = "Publish")]
        public Boolean IsPublished { get; set; } = false;

        public string CreatedBy { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set;} = DateTime.Now;
    }
}
