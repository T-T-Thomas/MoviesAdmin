using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        public int Id { get; set; }

        public int MovieId {  get; set; }

        [ValidateNever]
        public Movie Movie { get; set; } = null!;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public float Rating { get; set; }

        [Required]
        public Boolean IsPublished { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set;}
    }
}
