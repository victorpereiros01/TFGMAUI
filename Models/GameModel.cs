namespace TFGMaui.Models
{
    public class GameModel : HobbieModel
    {
        public int Id { get; set; } //

        public List<int> AgeRatings { get; set; }   //

        public double Aggregated_Rating { get; set; }   //

        public double Rating { get; set; }  //

        public int Category { get; set; }   //

        public int Cover { get; set; }  //

        public List<int> ExternalGames { get; set; }

        public long FirstReleaseDate { get; set; }  //

        public List<int> GameModes { get; set; }    //

        public List<int> Genres { get; set; }   //

        public List<int> InvolvedCompanies { get; set; }    //

        public string Name { get; set; }    //

        public List<int> Platforms { get; set; }    //

        public List<int> Screenshots { get; set; }  //

        public List<int> SimilarGames { get; set; }     //

        public string Storyline { get; set; }   //

        public string Summary { get; set; }     //

        public List<int> Themes { get; set; }       //

        public string Url { get; set; }     //

        public List<int> GameLocalizations { get; set; }

        public int? Collection { get; set; }

        public int? Status { get; set; }    //

        public List<int> Collections { get; set; }      //

        public List<int> Artworks { get; set; } //
    }
}
