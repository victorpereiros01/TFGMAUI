using Newtonsoft.Json;

namespace TFGMaui.Models
{
    public class GameModel : HobbieModel
    {
        public string Id { get; set; } //

        public List<int> AgeRatings { get; set; }   //

        public double AggregatedRating { get; set; }   //

        public double Rating { get; set; }  //

        public int Category { get; set; }   //

        public int Cover { get; set; }  //

        public List<int> ExternalGames { get; set; }

        public long FirstReleaseDate { get; set; }  //

        public List<int> GameModes { get; set; }    //

        public List<int> Genres { get; set; }   //

        public List<int> InvolvedCompanies { get; set; }    //

        [JsonProperty("name")]
        public string Title { get; set; }    //

        public List<int> Platforms { get; set; }    //

        public List<int> Screenshots { get; set; }  //

        public List<int> SimilarGames { get; set; }     //

        public string Storyline { get; set; }   //

        public string Summary { get; set; }     //

        public List<int> Themes { get; set; }       //

        public string Url { get; set; }     //

        public List<int> GameLocalizations { get; set; }

        public string Slug { get; set; }

        public int? Collection { get; set; }

        public int Status { get; set; }    //
        public string StatusString { get; set; }    //

        public List<int> Collections { get; set; }      //

        public List<int> Artworks { get; set; } //
    }

    public class BearerModel
    {
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    public class CoverModel
    {
        public int Id { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string Url { get; set; }
    }
}
