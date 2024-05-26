using Newtonsoft.Json;

namespace TFGMaui.Models
{
    internal class Page
    {
        public int PageC { get; set; }
        public List<MovieModel> Results { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
    }

    internal class PageAM
    {
        public Pagination Pagination { get; set; }
        public List<AnimeModel> Data { get; set; }
    }

    internal class Pagination
    {
        [JsonProperty("last_visible_page")]
        public int LastVisiblePage { get; set; }

        [JsonProperty("has_next_page")]
        public bool HasNextPage { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        public Items Items { get; set; }
    }

    internal class Items
    {
        public int Count { get; set; }
        public int Total { get; set; }
        [JsonProperty("per_page")]
        public int PerPage { get; set; }
    }

    public class HobbieModel
    {
        public string Imagen { get; set; }

        public bool IsChecked { get; set; }

        public string NombreHobbie { get; set; }
    }
}
