namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public double RunTime { get; set; } //126 minutes or 2h 6m

        public string Genre { get; set; } = string.Empty; //e.g horror or action

        public string AgeRating { get; set; } = string.Empty; //e.g PG-13

        public DateTime ReleaseDate { get; set; } //e.g August 24,2026

        public string PosterImage = string.Empty; //poster of the movie e.g matrix.png
    }
}
