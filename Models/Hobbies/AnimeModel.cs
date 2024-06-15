using Newtonsoft.Json;

namespace TFGMaui.Models
{
    public class AnimeModel : HobbieModel
    {
        [JsonProperty("mal_id")]
        public string Id { get; set; } // v

        public Imagen Images { get; set; }  // v

        public string? Title { get; set; }   // v

        public string? Type { get; set; }    // v

        public int? Episodes { get; set; }   // v

        public string? Status { get; set; }  // v

        public AiredIn? Aired { get; set; } // v

        public int? Year { get; set; }   // v

        public string? Rating { get; set; }  // v

        public string? Duration { get; set; }    // v

        public List<Parameters>? Studios { get; set; } // v

        public List<Parameters>? Genres { get; set; } // v

        public List<Parameters>? Demographics { get; set; }   // v

        public string? Synopsis { get; set; }    // v

        public string? Rank { get; set; }    // v

        public double? Score { get; set; }   // v
    }

    public class AnimeData
    {
        public AnimeModel Data { get; set; }
    }

    public class AnimeList
    {
        public List<AnimeModel> DataList { get; set; }
    }

    public class AiredIn
    {
        public string String { get; set; }
    }

    public class Imagen
    {
        public JpgInfo Jpg;
    }

    public class Parameters
    {
        public int Mal_Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class JpgInfo
    {
        public string Image_url { get; set; }
    }
}