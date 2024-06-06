using Newtonsoft.Json;

namespace TFGMaui.Models
{
    public class MangaModel : HobbieModel
    {
        [JsonProperty("mal_id")]
        public string Id { get; set; } // -

        public new Imagen Images { get; set; }  // -

        public string Title { get; set; }   // -

        public string Type { get; set; }    // -

        public string Status { get; set; }  // -

        public string Rank { get; set; }    // -

        public AiredIn Aired { get; set; }  // -

        [JsonIgnore]
        public int Chapters { get; set; }   // -

        [JsonIgnore]
        public int Volumes { get; set; }    // -

        public List<Parameters> Authors { get; set; }

        public List<Parameters> Genres { get; set; } // -

        public List<Parameters> Demographics { get; set; }   // -

        [JsonIgnore]
        public double Score { get; set; }   // -

        public string Synopsis { get; set; }    // -
    }

    public class MangaData
    {
        public MangaModel Data { get; set; }
    }
}
